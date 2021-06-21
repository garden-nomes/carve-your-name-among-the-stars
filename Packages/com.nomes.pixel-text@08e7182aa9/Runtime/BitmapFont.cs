using System.Collections.Generic;
using UnityEngine;

namespace Unity.PixelText
{

    [CreateAssetMenu(fileName = "New Bitmap Font", menuName = "Pixel Text/Bitmap Font")]
    public class BitmapFont : ScriptableObject
    {
        [Header("Font Sheet")]
        public Texture2D texture;
        public string ordering;
        public int gridWidth;
        public int gridHeight;

        [Header("Rendering")]
        public int spaceWidth = 4;
        public int letterSpacing = 1;
        public int lineSpacing = 1;

        private Dictionary<char, RectInt> _mappings;
        public Dictionary<char, RectInt> mappings => _mappings ?? (_mappings = GenerateMappings());

        private void OnValidate()
        {
            gridWidth = Mathf.Max(gridWidth, 0);
            gridHeight = Mathf.Max(gridHeight, 0);
            _mappings = GenerateMappings();
        }

        public bool isValid =>
            texture != null &&
            texture.isReadable &&
            ordering.Length > 0 &&
            gridWidth > 0 &&
            gridHeight > 0;

        private Dictionary<char, RectInt> GenerateMappings()
        {
            if (!isValid)
                return null;

            var result = new Dictionary<char, RectInt>();
            var pixels = texture.GetPixels();

            int x = 0;
            int y = texture.height - gridHeight;

            for (int i = 0; i < ordering.Length; i++)
            {

                int? left = null;
                int? right = null;

                for (int x0 = x; x0 < x + gridWidth; x0++)
                {
                    for (int y0 = y; y0 < y + gridHeight; y0++)
                    {
                        int pixelIndex = y0 * texture.width + x0;
                        if (pixels[pixelIndex].a > 0.2f)
                        {
                            if (left == null) left = x0;
                            if (right == null || right < x0) right = x0;
                        }
                    }
                }

                if (left == null || right == null)
                {
                    i--;
                }
                else
                {
                    char c = ordering[i];
                    int w = right.Value - left.Value;
                    var rect = new RectInt(left.Value, y, w + 1, gridHeight);
                    result.Add(c, rect);
                }

                if (x + gridWidth >= texture.width)
                {
                    if (y - gridHeight < 0)
                    {
                        break;
                    }

                    y -= gridHeight;
                    x = 0;
                }
                else
                {
                    x += gridWidth;
                }
            }

            return result;
        }

        public List<string> SplitIntoLines(string str, int width)
        {
            var lines = new List<string>();

            string line = "";
            string word = "";

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ' ')
                {
                    continue;
                }

                if (str[i] == '\n')
                {
                    var linePlusWord = line.Length > 0 ? $"{line} {word}" : word;

                    if (GetWidth(linePlusWord) > width)
                    {
                        lines.Add(line);
                        lines.Add(word);
                    }
                    else
                        lines.Add(linePlusWord);

                    line = "";
                    word = "";
                    continue;
                }

                word += str[i];

                if (i == str.Length - 1 || str[i + 1] == ' ')
                {
                    var linePlusWord = line.Length > 0 ? $"{line} {word}" : word;

                    if (GetWidth(linePlusWord) > width)
                    {
                        lines.Add(line);
                        line = word;
                        word = "";
                    }
                    else
                    {
                        line += line.Length > 0 ? $" {word}" : word;
                        word = "";
                    }
                }
            }

            lines.Add(line);
            return lines;
        }

        public int GetWidth(string str)
        {
            int width = 0;

            for (int i = 0; i < str.Length; i++)
            {
                width += GetWidth(str[i]);

                if (i != str.Length - 1 && str[i] != ' ' && str[i + 1] != ' ')
                {
                    width += letterSpacing;
                }
            }

            return width;
        }

        public int GetWidth(char letter)
        {
            if (letter == ' ' || !mappings.ContainsKey(letter))
            {
                return spaceWidth;
            }

            return mappings[letter].width;
        }

        public int GetHeight(string str, int width)
        {
            int lines = SplitIntoLines(str, width).Count;
            return lines * gridHeight + (lines - 1) * lineSpacing;
        }
    }
}
