using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [CustomEditor(typeof(NodeObject))]
    public class NodeObjectEditor : Editor
    {
        private SerializedProperty _nodeID;
        private SerializedProperty _nodeName;
        private SerializedProperty _nodeType;
        private SerializedProperty _bindingKey;
        private SerializedProperty _localizationKey;

        private void OnEnable()
        {
            _nodeID = serializedObject.FindProperty("_nodeID");
            _nodeName = serializedObject.FindProperty("_nodeName");
            _nodeType = serializedObject.FindProperty("_nodeType");
            _bindingKey = serializedObject.FindProperty("_bindingKey");
            _localizationKey = serializedObject.FindProperty("_localizationKey");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            LabelField(_nodeID);
            LabelField(_nodeName);
            LabelField(_nodeType);
            
            EditorGUILayout.Separator();
            LabelField(_bindingKey);
            LabelField(_localizationKey);

            /*
            var nodeObject = (NodeObject)target;
            _stylesFoldout = EditorGUILayout.Foldout(_stylesFoldout, "Styles");
            if (_stylesFoldout)
            {
                foreach (var style in nodeObject.styles)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ToggleLeft(GUIContent.none, style.enabled, GUILayout.Width(16));
                    EditorGUILayout.LabelField(style.selector, $"{style.GetType().Name}");
                    EditorGUILayout.EndHorizontal();
                }    
            }*/
        }

        private static void LabelField(SerializedProperty property)
        {
            var value = property.stringValue;
            if (!string.IsNullOrEmpty(value))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(property.displayName);
                EditorGUILayout.SelectableLabel(value, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField(property.displayName, "N/A", EditorStyles.miniLabel);
            }
        }
    }
}