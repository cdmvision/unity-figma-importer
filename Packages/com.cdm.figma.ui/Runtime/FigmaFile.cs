using System;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Cdm.Figma.UI
{
    public class FigmaFile : Figma.FigmaFile
    {
        [SerializeField]
        private FontSource[] _fonts = Array.Empty<FontSource>();

        /// <summary>
        /// Gets the font assets.
        /// </summary>
        public FontSource[] fonts
        {
            get => _fonts;
            set => _fonts = value;
        }

        [SerializeField]
        private TMP_FontAsset _fallbackFont;
        
        /// <summary>
        /// Gets or sets the fallback font that is used when a font mapping does not found.
        /// </summary>
        public TMP_FontAsset fallbackFont
        {
            get => _fallbackFont;
            set => _fallbackFont = value;
        }

        public override void MergeTo(Figma.FigmaFile otherFile, bool overwrite = false)
        {
            base.MergeTo(otherFile, overwrite);
            
            
            var other = (FigmaFile)otherFile;

            if (overwrite || other.fallbackFont == null)
            {
                other.fallbackFont = fallbackFont;
            }

            var oldFonts = other.fonts;
            
            // Deep copy font sources.
            other.fonts = new FontSource[fonts.Length];
            for (var i = 0; i < fonts.Length; i++)
            {
                other.fonts[i] = new FontSource(fonts[i]);

                if (!overwrite)
                {
                    var fontIndex = Array.FindIndex(oldFonts, 
                        x => x.fontName.Equals(other.fonts[i].fontName, StringComparison.OrdinalIgnoreCase));

                    if (fontIndex >= 0 && oldFonts[fontIndex].font != null)
                    {
                        other.fonts[i].font = oldFonts[fontIndex].font;
                    }
                }
            }
        }

        public bool TryGetFont(string fontName, out TMP_FontAsset font)
        {
            var fontIndex = 
                Array.FindIndex(fonts, x => string.Equals(x.fontName, fontName, StringComparison.OrdinalIgnoreCase));

            if (fontIndex >= 0 && fonts[fontIndex].font != null)
            {
                font = fonts[fontIndex].font;
                return true;
            }

            if (fallbackFont != null)
            {
                font = fallbackFont;
                return true;
            }

            font = null;
            return false;
        }
    }

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
        
        public static string GetFontName(string family, int weight, bool italic)
            => $"{family}-{(TextFontWeight) weight}{(italic ? "-Italic" : "")}";
    }
}