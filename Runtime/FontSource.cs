using System;
using TMPro;

namespace Cdm.Figma.UI
{
    [Serializable]
    public class FontSource
    {
        public string fontName;
        public TMP_FontAsset font;

        public FontSource()
        {
        }

        public FontSource(string fontName, TMP_FontAsset font)
        {
            this.fontName = fontName;
            this.font = font;
        }

        public FontSource(FontSource other)
        {
            this.fontName = other.fontName;
            this.font = other.font;
        }
    }
}