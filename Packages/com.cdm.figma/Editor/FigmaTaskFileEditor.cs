using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    public abstract class FigmaTaskFileEditor : Editor
    {
        private FigmaFile _selectedFile;
        private Editor _fileAssetEditor;
        private VisualElement _fileAssetElement;

        protected abstract string GetImplementationName();

        private void OnDisable()
        {
            if (_fileAssetEditor != null)
            {
                DestroyImmediate(_fileAssetEditor);
                _fileAssetEditor = null;
            }
        }

        protected virtual Task OnFigmaFileSaving(FigmaTaskFile taskFile, FigmaFile newFile, FigmaFile oldFile)
        {
            return Task.CompletedTask;
        }
        
        protected virtual Task OnFigmaFileSaved(FigmaTaskFile taskFile, FigmaFile file)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnFigmaFileImporting(FigmaTaskFile taskFile, FigmaFile file)
        {
            return Task.CompletedTask;
        }
        protected virtual Task OnFigmaFileImported(FigmaTaskFile taskFile, FigmaFile file)
        {
            return Task.CompletedTask;
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                $"{EditorHelper.VisualTreeFolderPath}/FigmaTaskFile.uxml");
            visualTree.CloneTree(root);

            root.Q<Label>("title").text = $"{GetImplementationName()} Figma Task File";
            
            root.Q<Button>("accessTokenHelpButton").clicked += () =>
            {
                Application.OpenURL("https://www.figma.com/developers/api#access-tokens");
            };
            
            root.Q<Button>("downloadFilesButton").clicked += async () =>
            {
                await GetFilesAsync((FigmaTaskFile) target);
            };
            
            root.Q<Button>("generateViewsButton").clicked += async () =>
            {
                await ImportFilesAsync((FigmaTaskFile) target);
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
            var assetPath = GetFigmaAssetPath((FigmaTaskFile) target, fileId);
            if (File.Exists(assetPath))
            {
                _selectedFile = AssetDatabase.LoadAssetAtPath<FigmaFile>(assetPath);

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

        private async Task ImportFilesAsync(FigmaTaskFile taskFile)
        {
            if (taskFile.GetImporter() == null)
            {
                Debug.LogError($"{nameof(IFigmaImporter)} cannot be empty.");
                return;
            }

            try
            {
                var fileCount = taskFile.fileIds.Count;
                for (var i = 0; i < fileCount; i++)
                {
                    var fileId = taskFile.fileIds[i];

                    EditorUtility.DisplayProgressBar("Importing Figma files", $"File: {fileId}", (float) i / fileCount);

                    var assetPath = GetFigmaAssetPath(taskFile, fileId);
                    if (File.Exists(assetPath))
                    {
                        var figmaFile = AssetDatabase.LoadAssetAtPath<FigmaFile>(assetPath);
                        if (figmaFile != null)
                        {
                            await OnFigmaFileImporting(taskFile, figmaFile);
                            await taskFile.GetImporter().ImportFileAsync(figmaFile);
                            await OnFigmaFileImported(taskFile, figmaFile);
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

                    EditorUtility.DisplayProgressBar("Importing Figma files", $"File: {fileId}",
                        (float) (i + 1) / fileCount);
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
        private async Task GetFilesAsync(FigmaTaskFile taskFile)
        {
            try
            {
                var fileCount = taskFile.fileIds.Count;
                for (var i = 0; i < fileCount; i++)
                {
                    var fileId = taskFile.fileIds[i];
                
                    EditorUtility.DisplayProgressBar("Downloading Figma files", $"File: {fileId}", (float) i / fileCount);
                    
                    // Save figma file asset.
                    var file = await DownloadAndSaveFigmaFileAsync(taskFile, fileId);
                    await OnFigmaFileSaved(taskFile, file);
                    
                    AssetDatabase.SaveAssetIfDirty(file);
                    EditorUtility.DisplayProgressBar("Downloading Figma files", $"File: {fileId}", (float) (i + 1) / fileCount);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private async Task<FigmaFile> DownloadAndSaveFigmaFileAsync(FigmaTaskFile taskFile, string fileID)
        {
            var newFile = await taskFile.GetDownloader().DownloadFileAsync(fileID, taskFile.personalAccessToken);
            
            var directory = Path.Combine("Assets", taskFile.assetsPath);
            Directory.CreateDirectory(directory);
            
            var figmaAssetPath = GetFigmaAssetPath(taskFile, fileID);

            var oldFile = AssetDatabase.LoadAssetAtPath<FigmaFile>(figmaAssetPath);
            var oldPages = oldFile != null ? oldFile.pages : Array.Empty<FigmaFilePage>();

            for (var i = 0; i < newFile.pages.Length; i++)
            {
                var oldPage = oldPages.FirstOrDefault(x => x.id == newFile.pages[i].id);
                if (oldPage != null)
                {
                    newFile.pages[i].enabled = oldPage.enabled;
                }
            }

            await OnFigmaFileSaving(taskFile, newFile, oldFile);
            
            AssetDatabase.DeleteAsset(figmaAssetPath);
            AssetDatabase.CreateAsset(newFile, figmaAssetPath);

            AssetDatabase.AddObjectToAsset(newFile.content, newFile);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newFile.content));

            if (newFile.thumbnail != null)
            {
                AssetDatabase.AddObjectToAsset(newFile.thumbnail, newFile);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newFile.thumbnail));    
            }
            
            AssetDatabase.Refresh();
            
            Debug.Log($"Figma file saved at: {figmaAssetPath}");
            return newFile;
        }
        
        private static string GetFigmaAssetPath(FigmaTaskFile taskFile, string fileId) 
            => Path.Combine("Assets", taskFile.assetsPath,  $"{fileId}.asset");
    }
}