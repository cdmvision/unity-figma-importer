using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;
using Debug = UnityEngine.Debug;

namespace Cdm.Figma.Editor
{
    [CustomEditor(typeof(FigmaDownloaderAsset), editorForChildClasses: true)]
    public class FigmaDownloaderAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty _personalAccessToken;
        private SerializedProperty _assetExtension;
        private SerializedProperty _assetPath;
        private SerializedProperty _downloadDependencies;
        private SerializedProperty _downloadImages;
        private SerializedProperty _fileId;
        private SerializedProperty _fileVersion;
        private SerializedProperty _fileName;
        private SerializedProperty _fileDefaultName;

        private ReorderableList _fileList;

        private bool _isDownloading = false;
        private bool _isDownloadingCompleted = false;
        private bool _isDownloadingDependency = false;
        private string _downloadingFile = "";
        private float _downloadingProgress = 0f;
        private CancellationTokenSource _cancellationTokenSource;

        protected virtual void OnEnable()
        {
            _personalAccessToken = serializedObject.FindProperty("_personalAccessToken");
            _assetPath = serializedObject.FindProperty("_assetPath");
            _assetExtension = serializedObject.FindProperty("_assetExtension");
            _downloadDependencies = serializedObject.FindProperty("_downloadDependencies");
            _downloadImages = serializedObject.FindProperty("_downloadImages");
            _fileId = serializedObject.FindProperty("_fileId");
            _fileVersion = serializedObject.FindProperty("_fileVersion");
            _fileName = serializedObject.FindProperty("_fileName");
            _fileDefaultName = serializedObject.FindProperty("_fileDefaultName");
        }

        public override bool RequiresConstantRepaint()
        {
            return _isDownloading || _isDownloadingCompleted || base.RequiresConstantRepaint();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($"Settings", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_personalAccessToken);
            if (GUILayout.Button("Click here for help generating access tokens", new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleRight,
                    hover = new GUIStyleState()
                    {
                        textColor = UnityEngine.Color.yellow
                    }
                }))
            {
                Application.OpenURL("https://www.figma.com/developers/api#access-tokens");
            }
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_fileId, new GUIContent("File ID"));
            EditorGUILayout.PropertyField(_fileName);
            EditorGUILayout.PropertyField(_fileVersion);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_assetPath);
            EditorGUILayout.PropertyField(_assetExtension);
            EditorGUILayout.PropertyField(_downloadDependencies);
            EditorGUILayout.PropertyField(_downloadImages);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Download", EditorStyles.miniButton))
            {
                _isDownloadingCompleted = false;
                _cancellationTokenSource = new CancellationTokenSource();
                DownloadFilesAsync();
            }

            EditorGUILayout.EndHorizontal();

            if (_isDownloading)
            {
                var description = !_isDownloadingDependency
                    ? $"File: {_downloadingFile}"
                    : $"File dependency: {_downloadingFile}";

                if (EditorUtility.DisplayCancelableProgressBar(
                        "Downloading Figma File", description, _downloadingProgress))
                {
                    _cancellationTokenSource.Cancel();
                }
            }
            else if (_isDownloadingCompleted)
            {
                _isDownloadingCompleted = false;

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawFileFields()
        {
            EditorGUILayout.PropertyField(_fileId);
            EditorGUILayout.PropertyField(_fileName);    
            
            /*if (!string.IsNullOrEmpty(_fileName.stringValue))
            {
                EditorGUILayout.PropertyField(_fileName);    
            }
            else
            {
                
            }*/
        }

        /*private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var file = _fileList.serializedProperty.GetArrayElementAtIndex(index);
            var fileId = file.FindPropertyRelative("id");
            var fileName = file.FindPropertyRelative("name");
            var defaultName = file.FindPropertyRelative("defaultName");

            var idRect = new Rect(rect.x, rect.y + 2, rect.width * 0.5f, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(idRect, fileId, GUIContent.none);

            if (string.IsNullOrEmpty(fileId.stringValue))
            {
                Placeholder(new Rect(idRect.x + 2, idRect.y, idRect.width - 2, idRect.height), "File ID");
            }

            var nameRect = new Rect(idRect.x + idRect.width + 2,
                rect.y + 2, rect.width - (idRect.width - 2), EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(nameRect, fileName, GUIContent.none);

            if (string.IsNullOrEmpty(fileName.stringValue))
            {
                var text = !string.IsNullOrEmpty(defaultName.stringValue)
                    ? defaultName.stringValue
                    : "File name (optional)";
                Placeholder(new Rect(nameRect.x + 2, nameRect.y, nameRect.width - 2, nameRect.height), text);
            }
        }*/

        private static void Placeholder(string value)
        {
            var guiColor = GUI.color;
            GUI.color = UnityEngine.Color.grey;
            EditorGUILayout.LabelField(value);
            GUI.color = guiColor;
        }

        private async void DownloadFilesAsync()
        {
            await DownloadFilesAsync((FigmaDownloaderAsset)target);
        }

        private async Task DownloadFilesAsync(FigmaDownloaderAsset downloader)
        {
            try
            {
                _isDownloading = true;
                _downloadingProgress = 0f;
                _downloadingFile = downloader.fileId;

                await DownloadAndSaveFigmaFileAsync(downloader, _cancellationTokenSource.Token);

                _downloadingProgress = 1f;
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Downloading cancelled.");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                _isDownloading = false;
                _isDownloadingCompleted = true;
                _isDownloadingDependency = false;
                _downloadingProgress = 0f;

                _cancellationTokenSource.Dispose();
            }
        }

        private async Task DownloadAndSaveFigmaFileAsync(
            FigmaDownloaderAsset downloader, CancellationToken cancellationToken)
        {
            var newFile = await downloader.GetDownloader()
                .DownloadFileAsync(downloader.personalAccessToken, downloader.fileId, downloader.fileVersion,
                    new Progress<FigmaDownloaderProgress>(
                        progress =>
                        {
                            _isDownloadingDependency = progress.isDependency;
                            _downloadingFile = progress.fileId;
                        }), cancellationToken);
            
            var directory = Path.Combine("Assets", downloader.assetPath);
            Directory.CreateDirectory(directory);

            downloader.defaultFileName = newFile.name;
            EditorUtility.SetDirty(downloader);

            var fileName = downloader.defaultFileName;
            if (!string.IsNullOrEmpty(downloader.fileName))
            {
                fileName = downloader.fileName;
            }

            var figmaAssetPath = GetFigmaAssetPath(downloader, fileName);

            // Save as compressed file.
            using var content = new MemoryStream(Encoding.UTF8.GetBytes(newFile.ToString("N")));
            await using var compressedFileStream = File.Create(figmaAssetPath);
            await using var compressor = new GZipStream(compressedFileStream, CompressionLevel.Optimal);
            await content.CopyToAsync(compressor, cancellationToken);

            //await File.WriteAllTextAsync(figmaAssetPath, newFile.ToString("N"), cancellationToken);

            Debug.Log($"Figma file saved at: {figmaAssetPath}");
        }

        private static string GetFigmaAssetPath(FigmaDownloaderAsset downloader, string fileName)
            => Path.Combine("Assets", downloader.assetPath, $"{fileName}.{downloader.assetExtension}");
    }
}