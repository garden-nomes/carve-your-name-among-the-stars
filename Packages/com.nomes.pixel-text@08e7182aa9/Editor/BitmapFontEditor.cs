using UnityEditor;
using UnityEngine;

namespace Unity.PixelText
{
    [CustomEditor(typeof(BitmapFont))]
    public class BitmapFontEditor : Editor
    {
        private string _previewText =>
            "The wizard quickly jinxed the gnomes before they vaporized.\n\n" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ\nabcdefghijklmnopqrstuvwxyz\n0123456789\n\n" +
            (target as BitmapFont).ordering;

        private static int _previewScale = 2;

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();

            var font = target as BitmapFont;

            if (font.texture != null && !font.texture.isReadable)
            {
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                GUILayout.BeginVertical(new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(16, 16, 16, 16)
                });

                EditorGUILayout.HelpBox("Can't read texture", MessageType.Error);

                GUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);

                GUILayout.Label(
                    "To use a texture as a font sheet, it needs \"Read/Write enabled\" checked " +
                    "in its import settings. Click the button below to enable this setting.",
                    new GUIStyle(GUI.skin.label) { wordWrap = true });

                GUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                bool button = GUILayout.Button(
                    "Update Import Settings",
                    new GUIStyle(GUI.skin.button) { padding = new RectOffset(16, 16, 8, 8) },
                    GUILayout.ExpandWidth(false));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (button) UpdateTextureImportSettings(font.texture);
                GUILayout.EndVertical();
            }
        }

        public override bool HasPreviewGUI() => (target as BitmapFont).isValid;
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            var font = target as BitmapFont;

            float padding = 4f;
            rect.xMin += padding;
            rect.yMin += padding;
            rect.xMax -= padding;
            rect.yMax -= padding;

            var glyphs = font.RenderText(
                _previewText, rect, _previewScale, TextAlign.Left, VerticalAlign.Top, Color.white);

            foreach (var glyph in glyphs)
            {
                var uv = glyph.uvRect;
                var dest = glyph.destinationRect;
                dest.y = rect.y + rect.height - (dest.y - font.gridHeight);

                GUI.DrawTextureWithTexCoords(dest, font.texture, uv);
            }
        }

        private void UpdateTextureImportSettings(Texture2D texture)
        {
            if (texture == null)
                return;

            string path = AssetDatabase.GetAssetPath(texture);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null)
                return;

            importer.isReadable = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Point;
            importer.npotScale = TextureImporterNPOTScale.None;

            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
