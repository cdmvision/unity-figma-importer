using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Cdm.Figma.Editor
{
    public abstract class FigmaAssetImporterBase : ScriptedImporter
    {
        protected const string DefaultExtension = "figma";
        protected const int ImportQueueOffset = 9999;

        protected const string FigmaIconColorPath = EditorHelper.PackageFolderPath +
                                                    "/Editor Default Resources/FigmaIcon-Color.png";

        protected const string FigmaIconFlatPath = EditorHelper.PackageFolderPath +
                                                   "/Editor Default Resources/FigmaIcon-Flat.png";

        [SerializeField]
        private FigmaFilePage[] _pages;

        /// <summary>
        /// Selected pages to be imported.
        /// </summary>
        public FigmaFilePage[] pages
        {
            get => _pages;
            set => _pages = value;
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            FigmaFile figmaFile;
            
            using (var compressedStream = File.Open(ctx.assetPath, FileMode.Open))
            {
                figmaFile = FigmaFile.ParseBinary(compressedStream);    
            }
            
            UpdatePages(figmaFile);

            var figmaImporter = GetFigmaImporter(ctx);
            OnAssetImporting(ctx, figmaImporter, figmaFile);

            var figmaDesign = figmaImporter.ImportFile(figmaFile, new IFigmaImporter.Options()
            {
                selectedPages = _pages.Where(p => p.enabled).Select(p => p.id).ToArray()
            });

            OnAssetImported(ctx, figmaImporter, figmaFile, figmaDesign);

            if (figmaDesign.thumbnail != null)
            {
                ctx.AddObjectToAsset("FigmaDesignPreview", figmaDesign.thumbnail);
            }

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(FigmaIconColorPath);
            ctx.AddObjectToAsset("FigmaDesign", figmaDesign.gameObject, icon);
            ctx.SetMainObject(figmaDesign.gameObject);
        }

        private void UpdatePages(FigmaFile file)
        {
            if (importSettingsMissing)
            {
                _pages = null;
            }

            var newPages = file.document.children ?? Array.Empty<PageNode>();
            newPages = newPages.Where(x => !x.IsIgnored()).ToArray();

            var oldPages = _pages;
            _pages = new FigmaFilePage[newPages.Length];

            for (var i = 0; i < _pages.Length; i++)
            {
                _pages[i] = new FigmaFilePage(newPages[i].id, newPages[i].name);

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

        protected virtual void OnAssetImporting(AssetImportContext ctx, IFigmaImporter figmaImporter,
            FigmaFile figmaFile)
        {
        }

        protected virtual void OnAssetImported(AssetImportContext ctx, IFigmaImporter figmaImporter,
            FigmaFile figmaFile, FigmaDesign figmaDesign)
        {
        }

        protected virtual string GetAssetExtension()
        {
            return DefaultExtension;
        }

        protected abstract IFigmaImporter GetFigmaImporter(AssetImportContext ctx);
    }

    [Serializable]
    public struct FigmaFilePage
    {
        public bool enabled;
        public string id;
        public string name;

        public FigmaFilePage(string id, string name)
        {
            this.id = id;
            this.name = name;
            this.enabled = true;
        }
    }
}