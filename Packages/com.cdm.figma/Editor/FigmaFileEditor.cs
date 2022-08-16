using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    [CustomEditor(typeof(FigmaFileAsset))]
    public class FigmaFileEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var fileAsset = ((FigmaFileAsset) target);

            var root = new VisualElement();
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{EditorHelper.VisualTreeFolderPath}/FigmaFile.uxml");
            visualTree.CloneTree(root);

            root.Q<Button>("copyContentButton").clicked += () =>
            {
                var assetPath = AssetDatabase.GetAssetPath(fileAsset);
                GUIUtility.systemCopyBuffer = File.ReadAllText(assetPath);
            };
            
            return root;
        }

        public override bool HasPreviewGUI()
        {
            var file = (FigmaFileAsset) target;
            return file != null && file.thumbnail != null;
        }

        public override void DrawPreview(Rect previewArea)
        {
            var thumbnail = ((FigmaFileAsset) target).thumbnail;

            var widthRatio = previewArea.width / thumbnail.width;
            var heightRatio = previewArea.height / thumbnail.height;
            var ratio = widthRatio > heightRatio ? heightRatio : widthRatio;
            
            var newWidth = thumbnail.width * ratio;
            var newHeight = thumbnail.height * ratio;
            var newX = previewArea.x + (previewArea.width - newWidth) * 0.5f;
            var newY = previewArea.y + (previewArea.height - newHeight) * 0.5f;
            
            var newPreviewArea = new Rect(newX, newY, newWidth, newHeight);
            GUI.DrawTexture(newPreviewArea, ((FigmaFileAsset) target).thumbnail);
        }
    }
}