using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    [CustomEditor(typeof(FigmaTaskFile), editorForChildClasses: true)]
    public class FigmaTaskFileEditor : Editor
    {
        private VisualElement _fileAssetElement;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                $"{EditorHelper.VisualTreeFolderPath}/FigmaTaskFile.uxml");
            visualTree.CloneTree(root);

            root.Q<Label>("title").text = $"Figma Task File";

            root.Q<Button>("accessTokenHelpButton").clicked += () =>
            {
                Application.OpenURL("https://www.figma.com/developers/api#access-tokens");
            };

            root.Q<Button>("downloadFilesButton").clicked += async () =>
            {
                await DownloadFilesAsync((FigmaTaskFile)target);
            };
            return root;
        }

        private async Task DownloadFilesAsync(FigmaTaskFile taskFile)
        {
            try
            {
                var fileCount = taskFile.files.Count;
                for (var i = 0; i < fileCount; i++)
                {
                    var fileId = taskFile.files[i];

                    EditorUtility.DisplayProgressBar("Downloading Figma files", $"File: {fileId}",
                        (float)i / fileCount);

                    // Save figma file asset.
                    var file = await DownloadAndSaveFigmaFileAsync(taskFile, fileId);

                    AssetDatabase.SaveAssetIfDirty(file);
                    EditorUtility.DisplayProgressBar("Downloading Figma files", $"File: {fileId}",
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

        private async Task<FigmaFile> DownloadAndSaveFigmaFileAsync(FigmaTaskFile taskFile, string fileID)
        {
            var newFile = await taskFile.GetDownloader().DownloadFileAsync(fileID, taskFile.personalAccessToken);

            var directory = Path.Combine("Assets", taskFile.assetsPath);
            Directory.CreateDirectory(directory);

            var figmaAssetPath = GetFigmaAssetPath(taskFile, fileID);

            if (File.Exists(figmaAssetPath))
                File.Delete(figmaAssetPath);

            await File.WriteAllTextAsync(figmaAssetPath, newFile.content.text);
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(figmaAssetPath);

            Debug.Log($"Figma file saved at: {figmaAssetPath}");
            return newFile;
        }

        private static string GetFigmaAssetPath(FigmaTaskFile taskFile, string fileId)
            => Path.Combine("Assets", taskFile.assetsPath, $"{fileId}.{taskFile.extension}");
    }
}