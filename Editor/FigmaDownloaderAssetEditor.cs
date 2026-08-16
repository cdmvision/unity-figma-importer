using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
        private SerializedProperty _branch;
        private SerializedProperty _branches;

        private bool _isDownloading = false;
        private bool _isDownloadingCompleted = false;
        private bool _isDownloadingDependency = false;
        private bool _isRefreshingBranches = false;
        private bool _isRefreshingBranchesCompleted = false;
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
            _branch = serializedObject.FindProperty("_branch");
            _branches = serializedObject.FindProperty("_branches");
        }

        public override bool RequiresConstantRepaint()
        {
            return _isDownloading || _isDownloadingCompleted ||
                   _isRefreshingBranches || _isRefreshingBranchesCompleted ||
                   base.RequiresConstantRepaint();
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

            DrawBranchInput();

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

            if (_isRefreshingBranches)
            {
                EditorUtility.DisplayProgressBar("Refresh Branches", "Fetching branches", 0f);
            }
            else if (_isRefreshingBranchesCompleted)
            {
                _isRefreshingBranchesCompleted = false;
                
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBranchInput()
        {
            EditorGUILayout.BeginHorizontal();

            var selectedIndex = 0;

            var options = new string[_branches.arraySize + 1];
            options[0] = "None";
            
            for (var i = 0; i < _branches.arraySize; i++)
            {
                var branch = _branches.GetArrayElementAtIndex(i);
                var branchKey = branch.FindPropertyRelative("key").stringValue;
                var branchName = branch.FindPropertyRelative("name").stringValue;
                options[i + 1] = branchName;

                if (branchKey == _branch.stringValue)
                {
                    selectedIndex = i + 1;
                }
            }
            
            var newSelectedIndex = EditorGUILayout.Popup(new GUIContent(_branch.displayName), selectedIndex, options);

            if (newSelectedIndex != selectedIndex)
            {
                if (newSelectedIndex > 0)
                {
                    var branch = _branches.GetArrayElementAtIndex(newSelectedIndex - 1);
                    var branchKey = branch.FindPropertyRelative("key").stringValue;
                    _branch.stringValue = branchKey;    
                }
                else
                {
                    _branch.stringValue = "";
                }
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh"), EditorStyles.miniButton, GUILayout.Width(32)))
            {
                RefreshBranchesAsync();
            }

            EditorGUILayout.EndHorizontal();
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

                var fileKey = downloader.fileId;
                var branch = _branch.stringValue;
                if (!string.IsNullOrEmpty(branch))
                {
                    fileKey = branch;
                }
                
                _downloadingFile = downloader.fileId;

                await DownloadAndSaveFigmaFileAsync(downloader, fileKey, _cancellationTokenSource.Token);

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

        private void UpdateBranches(FigmaFile file)
        {
            _branches.arraySize = 0;

            if (file.branches != null)
            {
                _branches.arraySize = file.branches.Length;

                for (var i = 0; i < file.branches.Length; i++)
                {
                    var branch = _branches.GetArrayElementAtIndex(i);
                    branch.FindPropertyRelative("key").stringValue = file.branches[i].key;
                    branch.FindPropertyRelative("name").stringValue = file.branches[i].name;
                }
                
                var isSelectedBranchExist = file.branches.Any(x => x.key == _branch.stringValue);
                if (!isSelectedBranchExist)
                {
                    _branch.stringValue = "";
                }
            }
            else
            {
                _branch.stringValue = "";
            }

            serializedObject.ApplyModifiedProperties();
        }

        private async void RefreshBranchesAsync()
        {
            try
            {
                _isRefreshingBranches = true;
                await RefreshBranchesAsync((FigmaDownloaderAsset) target);
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Operation cancelled.");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                _isRefreshingBranches = false;
                _isRefreshingBranchesCompleted = true;
            }
        }

        private async Task RefreshBranchesAsync(FigmaDownloaderAsset downloader, 
            CancellationToken cancellationToken = default)
        {
            var newFile = await downloader.GetDownloader()
                .DownloadFileAsync(downloader.personalAccessToken, downloader.fileId, downloader.fileVersion,
                    new Progress<FigmaDownloaderProgress>(
                        progress =>
                        {
                            _isDownloadingDependency = false;
                            _downloadingFile = progress.fileId;
                        }), cancellationToken);
            
            UpdateBranches(newFile);
        }

        private async Task DownloadAndSaveFigmaFileAsync(
            FigmaDownloaderAsset downloader, string fileKey, CancellationToken cancellationToken)
        {
            var newFile = await downloader.GetDownloader()
                .DownloadFileAsync(downloader.personalAccessToken, fileKey, downloader.fileVersion,
                    new Progress<FigmaDownloaderProgress>(
                        progress =>
                        {
                            _isDownloadingDependency = progress.isDependency;
                            _downloadingFile = progress.fileId;
                        }), cancellationToken);

            if (!newFile.IsBranch())
            {
                UpdateBranches(newFile);
            }
            
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