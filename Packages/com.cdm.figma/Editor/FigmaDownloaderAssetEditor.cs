using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
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
        private SerializedProperty _defaultFileName;

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
            _defaultFileName = serializedObject.FindProperty("_defaultFileName");
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
            DrawFileName();
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
        
        private void DrawFileName()
        {
            var rect = EditorGUILayout.GetControlRect(true);
            EditorGUI.PropertyField(rect, _fileName);
            
            if (string.IsNullOrEmpty(_fileName.stringValue))
            {
                var text = !string.IsNullOrEmpty(_defaultFileName.stringValue)
                    ? _defaultFileName.stringValue
                    : "File name (optional)";
                
                Placeholder(new Rect(
                    rect.x + EditorGUIUtility.labelWidth + 4, rect.y, 
                    rect.width - 4 - EditorGUIUtility.labelWidth, rect.height), text);
            }
        }

        private static void Placeholder(Rect rect, string value)
        {
            var guiColor = GUI.color;
            GUI.color = UnityEngine.Color.grey;
            EditorGUI.LabelField(rect, value);
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