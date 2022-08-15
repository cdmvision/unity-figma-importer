using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Cdm.Figma
{
    public abstract class FigmaAssetImporterBase<TFile> : ScriptedImporter where TFile : FigmaFile
    {
        protected const string Extension = "figma";
        
        [SerializeField]
        private List<bool> pageToggles;
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // Initialize parameters on first import.
            if (importSettingsMissing)
            {
                pageToggles = null;
            }

            var fileJson = File.ReadAllText(ctx.assetPath);
            var fileID = Path.GetFileNameWithoutExtension(ctx.assetPath);
            var file = FigmaFile.Create<TFile>(fileID, fileJson, null);
            
            ctx.AddObjectToAsset("Figma File", file, file.thumbnail);
            ctx.SetMainObject(file);
            
            // Update edges if source file has changed.
            if (pageToggles == null || pageToggles.Count != file.pages.Length)
            {
                pageToggles = new List<bool>();
                pageToggles.AddRange(file.pages.Select(x => x.enabled));
            }
            
            // Update reference object edge state.
            var pages = file.pages;

            for (var i = 0; i < pages.Length; i++)
            {
                pages[i].enabled = pageToggles[i];
            }

            var figmaImporter = GetFigmaImporter();
            figmaImporter.ImportFile(file);

            OnImportAsset(ctx, figmaImporter, file);
        }

        protected abstract void OnImportAsset(AssetImportContext ctx, IFigmaImporter figmaImporter, FigmaFile file);

        protected abstract IFigmaImporter GetFigmaImporter();
    }
}