using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    public class FigmaFileEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var fileAsset = ((FigmaFile) target);

            var root = new VisualElement();
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{EditorHelper.VisualTreeFolderPath}/FigmaFile.uxml");
            visualTree.CloneTree(root);
            
            var listItem =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorHelper.VisualTreeFolderPath}/FigmaFile_PageListItem.uxml");

            var listView = root.Q<ListView>("pagesList");
            listView.makeItem = () => listItem.Instantiate();
            listView.bindItem = (e, i) =>
            {
                var page = fileAsset.pages[i];
                var toggle = e.Q<Toggle>();
                toggle.label = page.name;
                toggle.value = page.enabled;
                toggle.RegisterValueChangedCallback(change =>
                {
                    page.enabled = change.newValue;
                    EditorUtility.SetDirty(target);
                });
            };

            root.Q<Button>("copyContentButton").clicked += () =>
            {
                if (fileAsset.content != null)
                {
                    GUIUtility.systemCopyBuffer = fileAsset.content.text;
                }
            };

            root.Q<Button>("exportFileButton").clicked += () =>
            {
                var path = EditorUtility.SaveFilePanel("Export Figma File", "", $"{fileAsset.id}.json", "json");

                if (!string.IsNullOrWhiteSpace(path))
                {
                    System.IO.File.WriteAllText(path, fileAsset.content.text);
                }
            };
            
            return root;
        }

        public override bool HasPreviewGUI()
        {
            var fileAsset = ((FigmaFile) target);
            return fileAsset != null && fileAsset.thumbnail != null;
        }

        public override void DrawPreview(Rect previewArea)
        {
            var thumbnail = ((FigmaFile) target).thumbnail;

            var widthRatio = previewArea.width / thumbnail.width;
            var heightRatio = previewArea.height / thumbnail.height;
            var ratio = widthRatio > heightRatio ? heightRatio : widthRatio;
            
            var newWidth = thumbnail.width * ratio;
            var newHeight = thumbnail.height * ratio;
            var newX = previewArea.x + (previewArea.width - newWidth) * 0.5f;
            var newY = previewArea.y + (previewArea.height - newHeight) * 0.5f;
            
            var newPreviewArea = new Rect(newX, newY, newWidth, newHeight);
            GUI.DrawTexture(newPreviewArea, ((FigmaFile) target).thumbnail);
        }
    }
}