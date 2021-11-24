using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    [CustomEditor(typeof(FigmaImporterTaskFile))]
    public class FigmaImporterEditor : Editor
    {
        private FigmaFileAsset _selectedFile;
        private Editor _fileAssetEditor;
        private VisualElement _fileAssetElement;
        private int _progressGetFilesId;
        private int _progressImportFilesId;

        private void OnDisable()
        {
            if (_fileAssetEditor != null)
            {
                DestroyImmediate(_fileAssetEditor);
                _fileAssetEditor = null;
            }
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PackageUtils.VisualTreeFolder}/FigmaImporter.uxml");
            visualTree.CloneTree(root);
            
            root.Q<Button>("accessTokenHelpButton").clicked += () =>
            {
                Application.OpenURL("https://www.figma.com/developers/api#access-tokens");
            };
            
            root.Q<Button>("downloadFilesButton").clicked += async () =>
            {
                await GetFilesAsync((FigmaImporterTaskFile) target);
            };
            
            root.Q<Button>("generateViewsButton").clicked += async () =>
            {
                await ImportFilesAsync((FigmaImporterTaskFile) target);
            };
            
            var fileListView = root.Q<ListView>("filesList");
            fileListView.onSelectionChange += objects =>
            {
                if (_fileAssetEditor != null)
                {
                    root.Remove(_fileAssetElement);
                    DestroyImmediate(_fileAssetEditor);
                }
                
                var selectedItem = (SerializedProperty) objects.LastOrDefault();
                if (selectedItem != null)
                {
                    PopulatePageList(root, selectedItem.stringValue);    
                }
            };
            return root;
        }

        private void PopulatePageList(VisualElement root, string fileId)
        {
            var assetPath = GetFigmaAssetPath((FigmaImporterTaskFile) target, fileId);
            if (System.IO.File.Exists(assetPath))
            {
                _selectedFile = AssetDatabase.LoadAssetAtPath<FigmaFileAsset>(assetPath);

                _fileAssetEditor = CreateEditor(_selectedFile);
                _fileAssetElement = _fileAssetEditor.CreateInspectorGUI();
                _fileAssetElement.Bind(_fileAssetEditor.serializedObject);
                
                root.Add(_fileAssetElement);
            }
        }
        
        
        public override bool HasPreviewGUI()
        {
            return _fileAssetEditor != null && _fileAssetEditor.HasPreviewGUI();
        }

        public override void DrawPreview(Rect previewArea)
        {
            _fileAssetEditor.DrawPreview(previewArea);
        }

        private async Task ImportFilesAsync(FigmaImporterTaskFile taskFile)
        {
            if (taskFile.importer == null)
            {
                Debug.LogError($"{nameof(FigmaImporter)} cannot be empty.");
                return;
            }

            try
            {
                _progressImportFilesId = Progress.Start($"Importing Figma files");
                
                var fileCount = taskFile.fileIds.Count;
                for (var i = 0; i < fileCount; i++)
                { 
                    var fileId = taskFile.fileIds[i];
                    Progress.Report(_progressImportFilesId, i, fileCount, $"File: {fileId}");

                    var assetPath = GetFigmaAssetPath(taskFile, fileId);
                    if (System.IO.File.Exists(assetPath))
                    {
                        var fileAsset = AssetDatabase.LoadAssetAtPath<FigmaFileAsset>(assetPath);
                        if (fileAsset != null)
                        {
                            await taskFile.importer.ImportFileAsync(fileAsset.GetFile());    
                        }
                        else
                        {
                            Debug.LogError($"File asset could not be loaded: {assetPath}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"File '{fileId}' asset does not exist. Please download it before importing.");   
                    }
                    
                    Progress.Report(_progressImportFilesId, i + 1, fileCount, $"File: {fileId}");
                }

                Progress.Finish(_progressImportFilesId);
            }
            catch (Exception e)
            {
                Progress.Finish(_progressImportFilesId, Progress.Status.Failed);
                Debug.LogError(e);
            }
        }
        
        private async Task GetFilesAsync(FigmaImporterTaskFile taskFile)
        {
            try
            {
                _progressGetFilesId = Progress.Start($"Downloading Figma files");
                
                var fileCount = taskFile.fileIds.Count;
                for (var i = 0; i < fileCount; i++)
                {
                    var fileId = taskFile.fileIds[i];
                
                    Progress.Report(_progressGetFilesId, i, fileCount, $"File: {fileId}");
                    
                    var fileContent = await FigmaApi.GetFileAsTextAsync(
                        new FigmaFileRequest(taskFile.personalAccessToken, fileId));

                    var file = FigmaFile.FromString(fileContent);
                    var thumbnail = await FigmaApi.GetThumbnailImageAsync(file.thumbnailUrl);
                    
                    SaveFigmaFile(taskFile, file, fileId, fileContent, thumbnail);
                
                    Progress.Report(_progressGetFilesId, i + 1, fileCount, $"File: {fileId}");
                }
                
                Progress.Finish(_progressGetFilesId);
            }
            catch (Exception e)
            {
                Progress.Finish(_progressGetFilesId, Progress.Status.Failed);
                Debug.LogError(e);
            }
        }
        
        private void SaveFigmaFile(FigmaImporterTaskFile taskFile, FigmaFile figmaFile, string fileId, 
            string fileContent, byte[] thumbnail)
        {
            var directory = Path.Combine("Assets", taskFile.assetsPath);
            Directory.CreateDirectory(directory);
            
            var figmaAsset = CreateInstance<FigmaFileAsset>();
            figmaAsset.id = fileId;
            figmaAsset.title = figmaFile.name;
            figmaAsset.version = figmaFile.version;
            figmaAsset.lastModified = figmaFile.lastModified.ToString("u");
            figmaAsset.content = new TextAsset(JObject.Parse(fileContent).ToString(Formatting.Indented));
            figmaAsset.content.name = "File";
            figmaAsset.thumbnail = new Texture2D(1, 1);
            figmaAsset.thumbnail.name = "Thumbnail";
            figmaAsset.thumbnail.LoadImage(thumbnail);
            
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

            var figmaAssetPath = GetFigmaAssetPath(taskFile, fileId);
            AssetDatabase.DeleteAsset(figmaAssetPath);
            AssetDatabase.CreateAsset(figmaAsset, figmaAssetPath);
            
            AssetDatabase.AddObjectToAsset(figmaAsset.content, figmaAsset);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(figmaAsset.content));
            
            AssetDatabase.AddObjectToAsset(figmaAsset.thumbnail, figmaAsset);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(figmaAsset.thumbnail));
            
            AssetDatabase.Refresh();
            
            Debug.Log($"Figma file saved at: {figmaAssetPath}");
        }
        
        private static string GetFigmaAssetPath(FigmaImporterTaskFile taskFile, string fileId) 
            => Path.Combine("Assets", taskFile.assetsPath,  $"{fileId}.asset");
    }
}