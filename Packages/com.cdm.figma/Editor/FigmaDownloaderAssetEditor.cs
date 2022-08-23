using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Cdm.Figma.Editor
{
    [CustomEditor(typeof(FigmaDownloaderAsset), editorForChildClasses: true)]
    public class FigmaDownloaderAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty _personalAccessToken;
        private SerializedProperty _assetExtension;
        private SerializedProperty _assetPath;
        private SerializedProperty _downloadDependencies;
        private SerializedProperty _files;
        
        private ReorderableList _fileList;

        private GUIStyle _linkStyle;
        
        protected virtual void OnEnable()
        {
            _personalAccessToken = serializedObject.FindProperty("_personalAccessToken");
            _assetPath = serializedObject.FindProperty("_assetPath");
            _assetExtension = serializedObject.FindProperty("_assetExtension");
            _downloadDependencies = serializedObject.FindProperty("_downloadDependencies");
            _files = serializedObject.FindProperty("_files");
            
            _fileList = new ReorderableList(serializedObject, _files, true, true, true, true);
            _fileList.drawHeaderCallback = DrawHeader;
            _fileList.drawElementCallback = DrawElement;

            _linkStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleRight,
                hover = new GUIStyleState()
                {
                    textColor = UnityEngine.Color.yellow
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($"Settings", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_personalAccessToken);
            if (GUILayout.Button("Click here for help generating access tokens", _linkStyle))
            {
                Application.OpenURL("https://www.figma.com/developers/api#access-tokens");
            }
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_assetPath);
            EditorGUILayout.PropertyField(_assetExtension);
            EditorGUILayout.PropertyField(_downloadDependencies);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            _fileList.DoLayoutList();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Download Files", EditorStyles.miniButton))
            {
                DownloadFilesAsync();
            }
            EditorGUILayout.EndHorizontal();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _files.displayName);
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var file = _fileList.serializedProperty.GetArrayElementAtIndex(index);
            var fileId = file.FindPropertyRelative("id");
            var fileName = file.FindPropertyRelative("name");

            var fileIdRect = new Rect(rect.x, rect.y + 2, rect.width * 0.5f, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(fileIdRect, fileId, GUIContent.none);
            
            var fileNameRect = new Rect(fileIdRect.x + fileIdRect.width + 2, 
                rect.y + 2, rect.width - (fileIdRect.width - 2), EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(fileNameRect, fileName, GUIContent.none);
        }

        private async void DownloadFilesAsync()
        {
            await DownloadFilesAsync((FigmaDownloaderAsset)target);
        }
        
        private static async Task DownloadFilesAsync(FigmaDownloaderAsset downloader)
        {
            try
            {
                var fileCount = downloader.files.Count;
                for (var i = 0; i < fileCount; i++)
                {
                    var file = downloader.files[i];

                    EditorUtility.DisplayProgressBar("Downloading Figma files", $"File: {file}", (float)i / fileCount);

                    // Save figma file asset.
                    await DownloadAndSaveFigmaFileAsync(downloader, file);

                    EditorUtility.DisplayProgressBar("Downloading Figma files", $"File: {file}", 
                        (float)(i + 1) / fileCount);
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

        private static async Task DownloadAndSaveFigmaFileAsync(FigmaDownloaderAsset downloader, 
            FigmaDownloaderAsset.File file)
        {
            var newFile = await downloader.GetDownloader().DownloadFileAsync(file.id, downloader.personalAccessToken);

            var directory = Path.Combine("Assets", downloader.assetPath);
            Directory.CreateDirectory(directory);

            if (string.IsNullOrEmpty(file.name))
            {
                file.name = newFile.name;
                EditorUtility.SetDirty(downloader);
            }
            
            var figmaAssetPath = GetFigmaAssetPath(downloader, file.name);
            await File.WriteAllTextAsync(figmaAssetPath, newFile.ToString("N"));
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(figmaAssetPath);

            Debug.Log($"Figma file saved at: {figmaAssetPath}");
        }

        private static string GetFigmaAssetPath(FigmaDownloaderAsset downloader, string fileId)
            => Path.Combine("Assets", downloader.assetPath, $"{fileId}.{downloader.assetExtension}");
    }
}