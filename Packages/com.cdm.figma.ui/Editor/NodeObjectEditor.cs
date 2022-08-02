using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [CustomEditor(typeof(NodeObject))]
    public class NodeObjectEditor : Editor
    {
        private bool _stylesFoldout = false;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawPropertiesExcluding(serializedObject, "_styles");

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
            }
        }
    }
}