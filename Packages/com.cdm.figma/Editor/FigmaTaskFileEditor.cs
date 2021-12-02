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
        
        private FigmaFileAsset _selectedFile;
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
        /// <param name="assetPath"></param>
        protected abstract void ImportGraphic(string assetPath);
        
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
                        var fileAsset = AssetDatabase.LoadAssetAtPath<FigmaFileAsset>(assetPath);
                        if (fileAsset != null)
                        {
                            var options = new FigmaImportOptions
                            {
                                pages = fileAsset.pages.Where(p => p.enabled).Select(p => p.id).ToArray(),
                                graphics = fileAsset.graphics,
                                fonts = fileAsset.fonts
                            };
                            await taskFile.GetImporter().ImportFileAsync(fileAsset.GetFile(), options);
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

                    var file = FigmaFile.FromString(fileContent);
                    var thumbnail = await FigmaApi.GetThumbnailImageAsync(file.thumbnailUrl);
                    
                    // Save figma file asset.
                    var fileAsset = SaveFigmaFile(taskFile, file, fileId, fileContent, thumbnail);
                    
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

        private static Task AddMissingFontsAsync(FigmaFile file, FigmaFileAsset fileAsset)
        {
            var fonts = new HashSet<FontDescriptor>();
            file.document.Traverse(node =>
            {
                fonts.Add(((TextNode) node).style.fontDescriptor);
                return true;
            }, NodeType.Text);

            foreach (var font in fonts)
            {
                fileAsset.fonts.Add(font, null);
            }
            
            EditorUtility.SetDirty(fileAsset);
            return Task.CompletedTask;
        }

        private async Task SaveVectorGraphicsAsync(
            FigmaTaskFile taskFile, FigmaFile file, string fileId, FigmaFileAsset fileAsset)
        {
            var nodeGraphicTypes = GetNodeGraphicTypes();
            var nodes = new List<VectorNode>();
            file.document.Traverse(node =>
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

                    ImportGraphic(graphicPath);

                    var graphicAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(graphicPath);
                    fileAsset.graphics.Add(graphic.Key, graphicAsset);
                }
                else
                {
                    Debug.LogWarning($"Graphic could not be rendered: {graphic.Key}");
                }
            }
            
            EditorUtility.SetDirty(fileAsset);
        }

        private static FigmaFileAsset SaveFigmaFile(FigmaTaskFile taskFile, FigmaFile figmaFile, string fileId, 
            string fileContent, byte[] thumbnail)
        {
            var directory = Path.Combine("Assets", taskFile.assetsPath);
            Directory.CreateDirectory(directory);
            
            var figmaAssetPath = GetFigmaAssetPath(taskFile, fileId);

            var oldFigmaAsset = AssetDatabase.LoadAssetAtPath<FigmaFileAsset>(figmaAssetPath);
            var oldPages = oldFigmaAsset != null ? oldFigmaAsset.pages : new FigmaFilePage[0];
            
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

                var oldPage = oldPages.FirstOrDefault(x => x.id == pages[i].id);
                if (oldPage != null)
                {
                    pages[i].enabled = oldPage.enabled;
                }
            }

            figmaAsset.pages = pages;

            
            AssetDatabase.DeleteAsset(figmaAssetPath);
            AssetDatabase.CreateAsset(figmaAsset, figmaAssetPath);
            
            AssetDatabase.AddObjectToAsset(figmaAsset.content, figmaAsset);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(figmaAsset.content));
            
            AssetDatabase.AddObjectToAsset(figmaAsset.thumbnail, figmaAsset);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(figmaAsset.thumbnail));
            
            AssetDatabase.Refresh();
            
            Debug.Log($"Figma file saved at: {figmaAssetPath}");
            return figmaAsset;
        }
        
        private static string GetFigmaAssetPath(FigmaTaskFile taskFile, string fileId) 
            => Path.Combine("Assets", taskFile.assetsPath,  $"{fileId}.asset");
    }
}