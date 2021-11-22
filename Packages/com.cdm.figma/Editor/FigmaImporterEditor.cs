using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    [CustomEditor(typeof(FigmaImporter))]
    public class FigmaImporterEditor : Editor
    {
        private const string VisualTreePath = "Packages/com.cdm.figma/Editor Default Resources/FigmaImporter.uxml";
        private int _progressId;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            visualTree.CloneTree(root);

            root.Q<Button>("accessTokenHelpButton").clicked += () =>
            {
                Application.OpenURL("https://www.figma.com/developers/api#access-tokens");
            };
            
            root.Q<Button>("downloadFilesButton").clicked += async () =>
            {
                await GetFilesAsync((FigmaImporter) target);
            };
            
            root.Q<Button>("generateViewsButton").clicked += () =>
            {
                Debug.Log("GENERATE VIEWS!!!!");
            };
            
            return root;
        }
        
        public async Task GetFilesAsync(FigmaImporter importer)
        {
            try
            {
                _progressId = Progress.Start($"Downloading Figma files");
                
                var fileCount = importer.files.Count;
                for (var i = 0; i < fileCount; i++)
                {
                    var fileId = importer.files[i];
                
                    Progress.Report(_progressId, i, fileCount, $"File: {fileId}");
                    
                    var fileContent = await FigmaApi.GetFileAsTextAsync(
                        new FigmaFileRequest(importer.personalAccessToken, fileId));
                    SaveFigmaFile(importer, fileId, fileContent);
                
                    Progress.Report(_progressId, i + 1, fileCount, $"File: {fileId}");
                }
                
                Progress.Finish(_progressId);
            }
            catch (Exception e)
            {
                Progress.Finish(_progressId, Progress.Status.Failed);
                Debug.LogError(e);
            }
        }
        
        private void SaveFigmaFile(FigmaImporter importer, string fileId, string fileContent)
        {
            var directory = Path.Combine("Assets", importer.assetsPath);
            Directory.CreateDirectory(directory);

            var figmaFile = FigmaFile.FromText(fileContent);

            var figmaAsset = CreateInstance<FigmaFileAsset>();
            figmaAsset.id = fileId;
            figmaAsset.title = figmaFile.name;
            figmaAsset.version = figmaFile.version;
            figmaAsset.lastModified = figmaFile.lastModified.ToString("u");
            figmaAsset.content = new TextAsset(JObject.Parse(fileContent).ToString(Formatting.Indented));
            figmaAsset.content.name = figmaFile.name ?? fileId;

            var canvases = figmaFile.document.children;
            var pages = new FigmaFilePage[canvases.Length];
            
            for (var i = 0; i < pages.Length; i++)
            {
                pages[i] = new FigmaFilePage()
                {
                    id = canvases[i].id,
                    name = canvases[i].name
                };
            }

            figmaAsset.pages = pages;

            var figmaAssetPath = Path.Combine(directory, $"{fileId}.asset");
            AssetDatabase.DeleteAsset(figmaAssetPath);
            AssetDatabase.CreateAsset(figmaAsset, figmaAssetPath);
            
            AssetDatabase.AddObjectToAsset(figmaAsset.content, figmaAsset);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(figmaAsset.content));
            
            AssetDatabase.Refresh();
            
            Debug.Log($"Figma file saved at: {figmaAssetPath}");
        }
    }
}