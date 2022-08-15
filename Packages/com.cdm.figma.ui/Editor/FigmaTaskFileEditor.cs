using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cdm.Figma.Utils;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [CustomEditor(typeof(FigmaTaskFile))]
    public class FigmaTaskFileEditor : Cdm.Figma.FigmaTaskFileEditor
    {
        protected override string GetImplementationName() => nameof(UI);
        
        protected override Task OnFigmaFileImported(Figma.FigmaTaskFile taskFile, Figma.FigmaFile file)
        {
            var figmaTaskFile = (FigmaTaskFile) taskFile;
            
            var documentsPath = figmaTaskFile.documentsPath;
            var assetsDirectory = Path.Combine("Assets", documentsPath);
            Directory.CreateDirectory(assetsDirectory);
            
            var documents = figmaTaskFile.importer.GetImportedDocuments();
            foreach (var document in documents)
            {
                try
                {
                    // Save generated resources.
                    /*var resources = CreateInstance<DocumentResources>();
                    resources.sprites = document.sprites.ToArray();
                    
                    var graphicsDirectory = Path.Combine("Assets", figmaTaskFile.graphicsPath);
                    var assetName = $"{NodeUtils.HyphenateNodeID(document.page.id)}.asset";
                    var assetPath = Path.Combine(graphicsDirectory, $"{assetName}");
                    AssetDatabase.CreateAsset(resources, assetPath);


                    foreach (var sprite in resources.sprites)
                    {
                        AssetDatabase.AddObjectToAsset(sprite, resources);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(sprite));
                    }
                    
                    foreach (var material in document.materials)
                    {
                        
                    }
                    
                    var prefabPath = Path.Combine(assetsDirectory, $"{document.page.name}.prefab");
                
                    if (File.Exists(prefabPath))
                        File.Delete(prefabPath);
                
                    var prefab = document.nodeObject.gameObject;
                    PrefabUtility.SaveAsPrefabAssetAndConnect(prefab, prefabPath, InteractionMode.AutomatedAction,
                        out var success);

                    if (success)
                    {
                        Debug.Log($"Prefab was saved: {prefabPath}");
                    }
                    else
                    {
                        Debug.LogError($"Prefab failed to save: {prefabPath}");
                    }*/
                }
                finally
                {
                    DestroyImmediate(document.nodeObject.gameObject);
                }
            }
            
            return Task.CompletedTask;
        }
    }
}
