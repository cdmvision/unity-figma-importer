using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.Editor
{
    [CustomEditor(typeof(FigmaDownloaderAsset), editorForChildClasses: true)]
    public class FigmaDownloaderAssetEditor : UnityEditor.Editor
    {
        private VisualElement _fileAssetElement;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                $"{EditorHelper.VisualTreeFolderPath}/FigmaDownloader.uxml");
            visualTree.CloneTree(root);

            root.Q<Button>("accessTokenHelpButton").clicked += () =>
            {
                Application.OpenURL("https://www.figma.com/developers/api#access-tokens");
            };

            root.Q<Button>("downloadFilesButton").clicked += async () =>
            {
                await DownloadFilesAsync((FigmaDownloaderAsset)target);
            };
            return root;
        }

        private async Task DownloadFilesAsync(FigmaDownloaderAsset downloader)
        {
            try
            {
                var fileCount = downloader.files.Count;
                for (var i = 0; i < fileCount; i++)
                {
                    var fileId = downloader.files[i];

                    EditorUtility.DisplayProgressBar("Downloading Figma files", $"File: {fileId}",
                        (float)i / fileCount);

                    // Save figma file asset.
                    await DownloadAndSaveFigmaFileAsync(downloader, fileId);

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

        private static async Task DownloadAndSaveFigmaFileAsync(FigmaDownloaderAsset downloader, string fileId)
        {
            var newFile = await downloader.GetDownloader().DownloadFileAsync(fileId, downloader.personalAccessToken);

            var directory = Path.Combine("Assets", downloader.assetPath);
            Directory.CreateDirectory(directory);

            var figmaAssetPath = GetFigmaAssetPath(downloader, fileId);
            await File.WriteAllTextAsync(figmaAssetPath, newFile.ToString("N"));
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(figmaAssetPath);

            Debug.Log($"Figma file saved at: {figmaAssetPath}");
        }

        private static string GetFigmaAssetPath(FigmaDownloaderAsset downloader, string fileId)
            => Path.Combine("Assets", downloader.assetPath, $"{fileId}.{downloader.assetExtension}");
    }
}