using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.PixelText
{
    public static class BitmapFontRenderingExtensions
    {
        public struct RenderedGlyph
        {
            public RenderedGlyph(Rect uvRect, Rect destinationRect)
            {
                this.uvRect = uvRect;
                this.destinationRect = destinationRect;
            }

            public Rect uvRect;
            public Rect destinationRect;
        }

        public static RenderedGlyph[] RenderText(
            this BitmapFont font,
            string text,
            Rect bounds,
            float scale,
            TextAlign align,
            VerticalAlign valign,
            Color color)
        {
            if (font == null || !font.isValid)
                return new RenderedGlyph[0];

            var pixelWidth = Mathf.FloorToInt(bounds.width / scale);
            var pixelHeight = Mathf.FloorToInt(bounds.height / scale);

            var lines = font.SplitIntoLines(text, pixelWidth);
            int height = lines.Count * font.gridHeight + (lines.Count - 1) * font.lineSpacing;

            int y;

            if (valign == VerticalAlign.Top)
                y = pixelHeight - font.gridHeight;
            else if (valign == VerticalAlign.Middle)
                y = (pixelHeight + height) / 2 - font.gridHeight;
            else
                y = height - font.gridHeight;

            var glyphs = new List<RenderedGlyph>();

            foreach (var line in lines)
            {
                int x;

                if (align == TextAlign.Right)
                    x = pixelWidth - font.GetWidth(line);
                else if (align == TextAlign.Center)
                    x = (pixelWidth - font.GetWidth(line)) / 2;
                else
                    x = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == ' ' || !font.mappings.ContainsKey(line[i]))
                    {
                        x += font.spaceWidth;
                    }
                    else
                    {
                        var pixelRect = font.mappings[line[i]];

                        var uvRect = new Rect(
                            (float) pixelRect.xMin / font.texture.width,
                            (float) pixelRect.yMin / font.texture.height,
                            (float) pixelRect.width / font.texture.width,
                            (float) pixelRect.height / font.texture.height);

                        var position = bounds.min + new Vector2(x * scale, y * scale);
                        var size = new Vector2(pixelRect.width * scale, pixelRect.height * scale);
                        var destinationRect = new Rect(position, size);

                        if (bounds.Overlaps(destinationRect))
                            glyphs.Add(new RenderedGlyph(uvRect, destinationRect));

                        x += pixelRect.width;
                    }

                    if (i != line.Length - 1 && line[i] != ' ' && line[i + 1] != ' ')
                        x += font.letterSpacing;
                }

                y -= font.gridHeight + font.lineSpacing;
            }

            return glyphs.ToArray();
        }
    }
}
