using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [CustomEditor(typeof(FigmaNode), editorForChildClasses: true)]
    public class FigmaNodeEditor : UnityEditor.Editor
    {
        private SerializedProperty _nodeID;
        private SerializedProperty _nodeName;
        private SerializedProperty _nodeType;
        private SerializedProperty _bindingKey;
        private SerializedProperty _localizationKey;

        private bool isLogsExpanded
        {
            get => EditorPrefs.GetBool($"{nameof(FigmaNodeEditor)}.{nameof(isLogsExpanded)}", false);
            set => EditorPrefs.SetBool($"{nameof(FigmaNodeEditor)}.{nameof(isLogsExpanded)}", value);
        }

        protected virtual void OnEnable()
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

            if (_localizationKey != null)
            {
                LabelField(_localizationKey);
            }

            EditorGUILayout.Separator();
            var figmaNode = (FigmaNode)target;
            if (figmaNode.logs.Any())
            {
                isLogsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isLogsExpanded, "Logs");
                if (isLogsExpanded)
                {
                    foreach (var log in figmaNode.logs)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(GetLogIcon(log.type), GUILayout.Width(16));
                        EditorGUILayout.LabelField(log.message, EditorStyles.miniLabel);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected static GUIContent GetLogIcon(FigmaImporterLogType type)
        {
            switch (type)
            {
                case FigmaImporterLogType.Info:
                    return EditorGUIUtility.IconContent("console.infoicon.sml");
                case FigmaImporterLogType.Warning:
                    return EditorGUIUtility.IconContent("console.warnicon.sml");
                case FigmaImporterLogType.Error:
                    return EditorGUIUtility.IconContent("console.erroricon.sml");
                default:
                    return GUIContent.none;
            }
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