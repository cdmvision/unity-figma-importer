using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [ScriptedImporter(1, Extension)]
    public class FigmaAssetImporter : FigmaAssetImporterBase<FigmaFile>
    {
        protected override void OnImportAsset(AssetImportContext ctx, IFigmaImporter figmaImporter, 
            Figma.FigmaFile file)
        {
            var documents = ((FigmaImporter) figmaImporter).GetImportedDocuments();
            
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
                
                ctx.AddObjectToAsset($"{document.page.id}", document.nodeObject.gameObject);

                /*var directory = Path.GetDirectoryName(assetPath) ?? "Assets";
                var pagePrefabPath = Path.Combine(directory, $"{pageInstance.name}.prefab");
                
                PrefabUtility.SaveAsPrefabAssetAndConnect(pageInstance, pagePrefabPath, InteractionMode.AutomatedAction, out var success);
                if (success)
                {
                    AssetDatabase.ImportAsset(pagePrefabPath);
                }
                else
                {
                    ctx.LogImportError("Prefab saving failed.");
                }
                
                DestroyImmediate(pageInstance);*/
            }
        }

        protected override IFigmaImporter GetFigmaImporter()
        {
            return new FigmaImporter();
        }
    }
}