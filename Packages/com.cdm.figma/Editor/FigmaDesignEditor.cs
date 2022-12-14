using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.Editor
{
    [CustomEditor(typeof(FigmaDesign), true)]
    public class FigmaDesignEditor : UnityEditor.Editor
    {
        private SerializedProperty _id;
        private SerializedProperty _title;
        private SerializedProperty _version;
        private SerializedProperty _lastModified;

        [MenuItem("CONTEXT/FigmaDesign/Copy File Content")]
        private static void CopyFileContent(MenuCommand command)
        {
            var figmaDesign = (FigmaDesign)command.context;
            var assetPath = AssetDatabase.GetAssetPath(figmaDesign);
            var json = File.ReadAllText(assetPath);
            json = JObject.Parse(json).ToString(Formatting.Indented);
            GUIUtility.systemCopyBuffer = json;
            
            Debug.Log($"File '{figmaDesign.title}' contents copied to clipboard.");
        }

        protected virtual void OnEnable()
        {
            _id = serializedObject.FindProperty("_id");
            _title = serializedObject.FindProperty("_title");
            _version = serializedObject.FindProperty("_version");
            _lastModified = serializedObject.FindProperty("_lastModified");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var guidEnabled = GUI.enabled;
            GUI.enabled = true;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($"Figma Design", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            LabelField(_id.displayName, _id.stringValue);
            LabelField(_title.displayName, _title.stringValue);
            LabelField(_version.displayName, _version.stringValue);
            LabelField(_lastModified.displayName, DateTime.Parse(_lastModified.stringValue).ToString("F"));
            
            GUI.enabled = guidEnabled;

            serializedObject.ApplyModifiedProperties();
        }

        public override bool HasPreviewGUI()
        {
            var file = (FigmaDesign)target;
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

        protected static void LabelField(string label, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(label);
                EditorGUILayout.SelectableLabel(value, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField(label, value);
            }
        }
    }
}