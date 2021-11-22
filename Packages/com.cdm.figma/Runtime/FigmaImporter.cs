using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Cdm.Figma
{
    [CreateAssetMenu(fileName = nameof(FigmaImporter), menuName = "Cdm/Figma/Figma Importer")]
    public class FigmaImporter : ScriptableObject
    {
        [SerializeField]
        private string _personalAccessToken;

        public string personalAccessToken
        {
            get => _personalAccessToken;
            set => _personalAccessToken = value;
        }

        [SerializeField]
        private string _assetsPath = "Resources/Figma/Files";

        public string assetsPath => _assetsPath;
        
        [SerializeField]
        private List<string> _files = new List<string>();
        
        public List<string> files => _files;
        
        public async Task GetFilesAsync()
        {
            foreach (var fileId in files)
            {
                Debug.Log($"File: {fileId} downloading...");
                
                var fileContent = await FigmaApi.GetFileAsTextAsync(new FigmaFileRequest(personalAccessToken, fileId));
                SaveFigmaFile(fileId, fileContent);
                
                Debug.Log($"File: {fileId} retrieved!");
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void SaveFigmaFile(string fileId, string fileContent)
        {
            var directory = Path.Combine("Assets", assetsPath);
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
            UnityEditor.AssetDatabase.DeleteAsset(figmaAssetPath);
            UnityEditor.AssetDatabase.CreateAsset(figmaAsset, figmaAssetPath);
            
            UnityEditor.AssetDatabase.AddObjectToAsset(figmaAsset.content, figmaAsset);
            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(figmaAsset.content));
            
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}