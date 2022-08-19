using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.Editor
{
    [CustomEditor(typeof(FigmaDesign), true)]
    public class FigmaDesignEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var fileAsset = ((FigmaDesign) target);

            var root = new VisualElement();
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{EditorHelper.VisualTreeFolderPath}/FigmaDesign.uxml");
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
            var file = (FigmaDesign) target;
            return file != null && file.thumbnail != null;
        }
        
        public override void OnPreviewGUI(Rect previewArea, GUIStyle background)
        {
            base.OnPreviewGUI(previewArea, background);

            var figmaAsset = (FigmaDesign)target;
            var thumbnail = figmaAsset.thumbnail;

            var widthRatio = (previewArea.width - 8f) / thumbnail.width;
            var heightRatio = (previewArea.height - 8f) / thumbnail.height;
            var ratio = widthRatio > heightRatio ? heightRatio : widthRatio;
            
            var newWidth = thumbnail.width * ratio;
            var newHeight = thumbnail.height * ratio;
            var newX = previewArea.x + (previewArea.width - newWidth) * 0.5f;
            var newY = previewArea.y + (previewArea.height - newHeight) * 0.5f;
            
            var newPreviewArea = new Rect(newX, newY, newWidth, newHeight);
            GUI.DrawTexture(newPreviewArea, thumbnail);
            
            const float labelHeight = 40;
            EditorGUI.DropShadowLabel(
                new Rect(previewArea.x, previewArea.height - labelHeight - 6, previewArea.width, labelHeight), 
                $"{figmaAsset.title} - {DateTime.Parse(figmaAsset.lastModified):G}");
        }
    }
}