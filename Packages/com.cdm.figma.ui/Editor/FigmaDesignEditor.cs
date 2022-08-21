using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [CustomEditor(typeof(FigmaDesign), true)]
    public class FigmaDesignEditor : Figma.Editor.FigmaDesignEditor
    {
        private SerializedProperty _bindings;

        private bool isBindingsExpanded
        {
            get => EditorPrefs.GetBool($"{nameof(FigmaDesignEditor)}.{nameof(isBindingsExpanded)}", false);
            set => EditorPrefs.SetBool($"{nameof(FigmaDesignEditor)}.{nameof(isBindingsExpanded)}", value);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _bindings = serializedObject.FindProperty("_bindings");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var guidEnabled = GUI.enabled;
            GUI.enabled = true;
            
            if (_bindings.arraySize > 0)
            {
                EditorGUILayout.Separator();
                isBindingsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isBindingsExpanded, _bindings.displayName);
                if (isBindingsExpanded)
                {
                    for (var i = 0; i < _bindings.arraySize; i++)
                    {
                        var binding = _bindings.GetArrayElementAtIndex(i);
                        var key = binding.FindPropertyRelative("_key");
                        var path = binding.FindPropertyRelative("_path");
                        var node = binding.FindPropertyRelative("_node");

                        EditorGUILayout.BeginHorizontal();
                        
                        if (GUILayout.Button(key.stringValue, EditorStyles.miniButtonLeft, GUILayout.Width(196)))
                        {
                            GUIUtility.systemCopyBuffer = key.stringValue;
                        }
                        
                        var nodeRef = (FigmaNode) node.objectReferenceValue;
                        var nodeLabel = nodeRef != null ? nodeRef.nodeName : "N/A";
                        if (GUILayout.Button(new GUIContent(nodeLabel, path.stringValue), EditorStyles.objectField))
                        {
                            GUIUtility.systemCopyBuffer = path.stringValue;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            
            GUI.enabled = guidEnabled;
        }
    }
}