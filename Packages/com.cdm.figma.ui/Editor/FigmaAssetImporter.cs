using System;
using TMPro;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [ScriptedImporter(1, Extension)]
    public class FigmaAssetImporter : FigmaAssetImporterBase
    {
        [SerializeField]
        private FontSource[] _fonts;

        /// <summary>
        /// Gets the font assets.
        /// </summary>
        public FontSource[] fonts => _fonts;

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

        protected override void OnAssetImporting(AssetImportContext ctx, IFigmaImporter figmaImporter, FigmaFile file)
        {
            base.OnAssetImporting(ctx, figmaImporter, file);

            UpdateFonts((FigmaImporter)figmaImporter, file);
        }

        protected override void OnAssetImported(AssetImportContext ctx, IFigmaImporter figmaImporter, FigmaFile file)
        {
            base.OnAssetImported(ctx, figmaImporter, file);

            var documents = ((FigmaImporter)figmaImporter).GetImportedDocuments();

            foreach (var document in documents)
            {
                foreach (var sprite in document.sprites)
                {
                    ctx.AddObjectToAsset(sprite.name, sprite);
                    ctx.AddObjectToAsset($"{sprite.name}_tex", sprite.texture);
                }

                foreach (var material in document.materials)
                {
                    ctx.AddObjectToAsset(material.name, material);
                }

                ctx.AddObjectToAsset($"{document.pageNode.id}", document.pageNodeObject.gameObject);
            }
        }

        protected override IFigmaImporter GetFigmaImporter()
        {
            return new FigmaImporter();
        }

        private void UpdateFonts(FigmaImporter figmaImporter, FigmaFile file)
        {
            if (importSettingsMissing)
            {
                _fonts = null;
            }

            var usedFonts = file.GetUsedFonts();
            var oldFonts = _fonts;
            _fonts = new FontSource[usedFonts.Length];

            for (var i = 0; i < _fonts.Length; i++)
            {
                _fonts[i] = new FontSource(usedFonts[i], null);

                // Restore previously assigned fonts.
                if (oldFonts != null)
                {
                    var oldFontIndex = Array.FindIndex(oldFonts, x => x.fontName == _fonts[i].fontName);
                    if (oldFontIndex >= 0)
                    {
                        _fonts[i].font = oldFonts[oldFontIndex].font;
                    }
                }
            }

            figmaImporter.fonts.AddRange(_fonts);
            figmaImporter.fallbackFont = fallbackFont;
        }
    }
}