using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [CustomEditor(typeof(FigmaNode), editorForChildClasses: true)]
    public class FigmaNodeEditor : UnityEditor.Editor
    {
        private SerializedProperty _nodeId;
        private SerializedProperty _nodeName;
        private SerializedProperty _nodeType;
        private SerializedProperty _bindingKey;
        private SerializedProperty _localizationKey;
        private SerializedProperty _tags;

        private bool isLogsExpanded
        {
            get => EditorPrefs.GetBool($"{nameof(FigmaNodeEditor)}.{nameof(isLogsExpanded)}", false);
            set => EditorPrefs.SetBool($"{nameof(FigmaNodeEditor)}.{nameof(isLogsExpanded)}", value);
        }

        protected virtual void OnEnable()
        {
            _nodeId = serializedObject.FindProperty("_nodeId");
            _nodeName = serializedObject.FindProperty("_nodeName");
            _nodeType = serializedObject.FindProperty("_nodeType");
            _bindingKey = serializedObject.FindProperty("_bindingKey");
            _localizationKey = serializedObject.FindProperty("_localizationKey");
            _tags = serializedObject.FindProperty("_tags");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            

            LabelField(_nodeId.displayName, _nodeId.stringValue);
            LabelField(_nodeName.displayName, _nodeName.stringValue);
            LabelField(_nodeType.displayName, _nodeType.stringValue);

            EditorGUILayout.Separator();
            LabelField(_bindingKey.displayName, _bindingKey.stringValue);

            if (_localizationKey != null)
            {
                LabelField(_localizationKey.displayName, _localizationKey.stringValue);
            }
            
            var figmaNode = (FigmaNode)target;
            
            LabelField(_tags.displayName, string.Join(", ", figmaNode.tags));

            EditorGUILayout.Separator();
            if (figmaNode.logs.Any())
            {
                isLogsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isLogsExpanded, "Logs");
                if (isLogsExpanded)
                {
                    foreach (var log in figmaNode.logs)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(GetLogIcon(log.type), GUILayout.Width(16), GUILayout.ExpandHeight(true));
                        EditorGUILayout.LabelField(log.message, EditorStyles.wordWrappedMiniLabel);
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

        private static void LabelField(string label, string value)
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
                EditorGUILayout.LabelField(label, "N/A", EditorStyles.miniLabel);
            }
        }
    }
}