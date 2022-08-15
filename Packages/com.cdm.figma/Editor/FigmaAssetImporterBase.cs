using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Cdm.Figma
{
    public abstract class FigmaAssetImporterBase : ScriptedImporter
    {
        protected const string Extension = "figma";

        [SerializeField]
        private FigmaFilePage[] _pages;

        /// <summary>
        /// All pages in the figma file.
        /// </summary>
        public FigmaFilePage[] pages => _pages;

        [SerializeField, HideInInspector]
        private FigmaFile _figmaFile;

        internal FigmaFile figmaFile => _figmaFile;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var fileJson = File.ReadAllText(ctx.assetPath);
            var fileID = Path.GetFileNameWithoutExtension(ctx.assetPath);
            var file = FigmaFile.Create<FigmaFile>(fileID, fileJson, null);
            
            UpdatePages(file);
            
            _figmaFile = file;
            
            ctx.AddObjectToAsset("Figma File", file, file.thumbnail);
            ctx.SetMainObject(file);
            
            for (var i = 0; i < pages.Length; i++)
            {
                file.pages[i].enabled = pages[i].enabled;
            }

            var figmaImporter = GetFigmaImporter();
            
            OnAssetImporting(ctx, figmaImporter, file);
            figmaImporter.ImportFile(file);
            OnAssetImported(ctx, figmaImporter, file);
        }

        private void UpdatePages(FigmaFile file)
        {
            if (importSettingsMissing)
            {
                _pages = null;
            }

            var oldPages = _pages;
            _pages = new FigmaFilePage[file.pages.Length];

            for (var i = 0; i < _pages.Length; i++)
            {
                _pages[i] = new FigmaFilePage(file.pages[i]);

                // Restore previously page status.
                if (oldPages != null)
                {
                    var oldPageIndex = Array.FindIndex(oldPages, x => x.id == _pages[i].id);
                    if (oldPageIndex >= 0)
                    {
                        _pages[i].enabled = oldPages[oldPageIndex].enabled;
                    }
                }
            }
        }

        protected virtual void OnAssetImporting(AssetImportContext ctx, IFigmaImporter figmaImporter, FigmaFile file)
        {
        }

        protected virtual void OnAssetImported(AssetImportContext ctx, IFigmaImporter figmaImporter, FigmaFile file)
        {
        }

        protected abstract IFigmaImporter GetFigmaImporter();
    }
}