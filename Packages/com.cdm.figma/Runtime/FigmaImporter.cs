using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
                
                var contents = await FigmaApi.GetFileAsTextAsync(new FigmaFileRequest(personalAccessToken, fileId));
                SaveFigmaFile(fileId, contents);
                
                Debug.Log($"File: {fileId} retrieved!");
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void SaveFigmaFile(string fileId, string contents)
        {
            var directory = Path.Combine("Assets", assetsPath);
            Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, $"{fileId}.json");
            
            System.IO.File.WriteAllText(filePath, contents);
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}