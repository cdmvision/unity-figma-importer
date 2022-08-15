using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditorInternal;
using UnityEngine;

namespace Cdm.Figma
{
    [CustomEditor(typeof(FigmaAssetImporterBase), editorForChildClasses: true)]
    public class FigmaAssetImporterBaseEditor : ScriptedImporterEditor
    {
        private SerializedProperty _pagesProperty;
        private ReorderableList _pagesList;

        public override void OnEnable()
        {
            base.OnEnable();

            _pagesProperty = serializedObject.FindProperty("_pages");
            _pagesList = new ReorderableList(serializedObject, _pagesProperty, false, true, false, false);
            _pagesList.drawHeaderCallback = DrawHeader;
            _pagesList.drawElementCallback = DrawElement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGUI();

            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Separator();
            ApplyRevertGUI();
        }

        protected virtual void DrawGUI()
        {
            EditorGUILayout.Separator();
            _pagesList.DoLayoutList();
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _pagesProperty.displayName);
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _pagesList.serializedProperty.GetArrayElementAtIndex(index);
            
            var enabledRect = new Rect(rect.x, rect.y + 2, 16, EditorGUIUtility.singleLineHeight);
            var enabledProperty = element.FindPropertyRelative("enabled");
            enabledProperty.boolValue = EditorGUI.ToggleLeft(enabledRect, GUIContent.none, enabledProperty.boolValue);

            var nameRect = new Rect(enabledRect.x + enabledRect.width + 2, rect.y + 2, 
                rect.width - enabledRect.width - 2, EditorGUIUtility.singleLineHeight);
            var nameProperty = element.FindPropertyRelative("name");
            EditorGUI.LabelField(nameRect, new GUIContent(nameProperty.stringValue));
        }
    }
}