using System;
using System.Collections.Generic;
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
    public abstract class FigmaTaskFileEditor : Editor
    {
        
        private FigmaFile _selectedFile;
        private Editor _fileAssetEditor;
        private VisualElement _fileAssetElement;

        protected abstract string GetImplementationName();
        
        /// <summary>
        /// Get node types that will be downloaded as graphic.
        /// </summary>
        /// <seealso cref="NodeType"/>
        protected abstract string[] GetNodeGraphicTypes();
        
        /// <summary>
        /// This is called after node graphic was downloaded.
        /// </summary>
        protected abstract void ImportGraphic(FigmaFile file, string graphicName, string graphicAsset);

        protected abstract void ImportFont(FigmaFile file, FontDescriptor fontDescriptor);
        
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
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                $"{PackageUtils.VisualTreeFolder}/FigmaTaskFile.uxml");
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
                Debug.LogError($"{nameof(FigmaImporter)} cannot be empty.");
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
                            await taskFile.GetImporter().ImportFileAsync(figmaFile);
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
                    
                    var fileContent = await FigmaApi.GetFileAsTextAsync(
                        new FigmaFileRequest(taskFile.personalAccessToken, fileId)
                        {
                            geometry = "paths",
                            plugins = new []{ PluginData.Id }
                        });

                    var file = FigmaFileContent.FromString(fileContent);
                    var thumbnail = await FigmaApi.GetThumbnailImageAsync(file.thumbnailUrl);
                    
                    // Save figma file asset.
                    var fileAsset = SaveFigmaFile(taskFile, fileId, fileContent, thumbnail);
                    
                    // Save Vector nodes as graphic asset.
                    await SaveVectorGraphicsAsync(taskFile, file, fileId, fileAsset);
                    await AddMissingFontsAsync(file, fileAsset);
                    
                    AssetDatabase.SaveAssetIfDirty(fileAsset);
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

        private Task AddMissingFontsAsync(FigmaFileContent fileContent, FigmaFile file)
        {
            var fonts = new HashSet<FontDescriptor>();
            fileContent.document.Traverse(node =>
            {
                fonts.Add(((TextNode) node).style.fontDescriptor);
                return true;
            }, NodeType.Text);

            foreach (var font in fonts)
            {
                ImportFont(file, font);
            }
            
            EditorUtility.SetDirty(file);
            return Task.CompletedTask;
        }

        private async Task SaveVectorGraphicsAsync(
            FigmaTaskFile taskFile, FigmaFileContent fileContent, string fileId, FigmaFile file)
        {
            var nodeGraphicTypes = GetNodeGraphicTypes();
            var nodes = new List<VectorNode>();
            fileContent.document.Traverse(node =>
            {
                var vectorNode = (VectorNode) node;
                if (vectorNode.visible)
                {
                    nodes.Add(vectorNode);    
                }
                return true;
            }, nodeGraphicTypes);

            if (!nodes.Any())
                return;
            
            var graphics = 
                await FigmaApi.GetImageAsync(new FigmaImageRequest(taskFile.personalAccessToken, fileId)
                {
                    ids = nodes.Select(x => x.id).ToArray(),
                    format = "svg",
                    svgIncludeId = false,
                    svgSimplifyStroke = true
                });
            
            var directory = Path.Combine("Assets", taskFile.graphicsPath);
            Directory.CreateDirectory(directory);

            foreach (var graphic in graphics)
            {
                if (graphic.Value != null)
                {
                    var fileName = $"{graphic.Key.Replace(":", "-").Replace(";", "_")}.svg";
                    
                    var path = Path.Combine(Application.dataPath, taskFile.graphicsPath, fileName);
                    await File.WriteAllBytesAsync(path, graphic.Value);

                    var graphicPath = Path.Combine("Assets", taskFile.graphicsPath, fileName);
                    AssetDatabase.ImportAsset(graphicPath, ImportAssetOptions.ForceSynchronousImport);

                    ImportGraphic(file, graphic.Key, graphicPath);
                }
                else
                {
                    Debug.LogWarning($"Graphic could not be rendered: {graphic.Key}");
                }
            }
            
            EditorUtility.SetDirty(file);
        }

        private static FigmaFile SaveFigmaFile(FigmaTaskFile taskFile, string fileId, string fileContent, byte[] thumbnail)
        {
            var directory = Path.Combine("Assets", taskFile.assetsPath);
            Directory.CreateDirectory(directory);
            
            var figmaAssetPath = GetFigmaAssetPath(taskFile, fileId);

            var oldFigmaAsset = AssetDatabase.LoadAssetAtPath<FigmaFile>(figmaAssetPath);
            var oldPages = oldFigmaAsset != null ? oldFigmaAsset.pages : new FigmaFilePage[0];

            var figmaFile = taskFile.GetImporter().CreateFile(fileId, fileContent, thumbnail);
            
            for (var i = 0; i < figmaFile.pages.Length; i++)
            {
                var oldPage = oldPages.FirstOrDefault(x => x.id == figmaFile.pages[i].id);
                if (oldPage != null)
                {
                    figmaFile.pages[i].enabled = oldPage.enabled;
                }
            }
            
            AssetDatabase.DeleteAsset(figmaAssetPath);
            AssetDatabase.CreateAsset(figmaFile, figmaAssetPath);

            AssetDatabase.AddObjectToAsset(figmaFile.content, figmaFile);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(figmaFile.content));

            if (figmaFile.thumbnail != null)
            {
                AssetDatabase.AddObjectToAsset(figmaFile.thumbnail, figmaFile);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(figmaFile.thumbnail));    
            }
            
            AssetDatabase.Refresh();
            
            Debug.Log($"Figma file saved at: {figmaAssetPath}");
            return figmaFile;
        }
        
        private static string GetFigmaAssetPath(FigmaTaskFile taskFile, string fileId) 
            => Path.Combine("Assets", taskFile.assetsPath,  $"{fileId}.asset");
    }
}