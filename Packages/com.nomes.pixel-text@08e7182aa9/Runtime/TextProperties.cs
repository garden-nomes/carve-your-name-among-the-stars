using System;
using UnityEngine;

namespace Unity.PixelText
{
    [Serializable]
    public class TextProperties
    {
        [SerializeField] BitmapFont _font;
        public BitmapFont font
        {
            get => _font;
            set => _font = value;
        }

        [SerializeField] float _scale;
        public float scale
        {
            get => _scale;
            set => _scale = value;
        }

        [SerializeField] TextAlign _horizontalAlign;
        public TextAlign horizontalAlign
        {
            get => _horizontalAlign;
            set => _horizontalAlign = value;
        }

        [SerializeField] VerticalAlign _verticalAlign;
        public VerticalAlign verticalAlign
        {
            get => _verticalAlign;
            set => _verticalAlign = value;
        }

        public static TextProperties defaultProperties => new TextProperties()
        {
            scale = 1f,
            horizontalAlign = TextAlign.Left,
            verticalAlign = VerticalAlign.Top,
        };
    }
}
