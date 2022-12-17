using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [CustomEditor(typeof(FigmaDocument), editorForChildClasses: true)]
    public class FigmaDocumentEditor : FigmaNodeEditor
    {
        private bool isAllLogsExpanded
        {
            get => EditorPrefs.GetBool($"{nameof(FigmaDocumentEditor)}.{nameof(isAllLogsExpanded)}", false);
            set => EditorPrefs.SetBool($"{nameof(FigmaDocumentEditor)}.{nameof(isAllLogsExpanded)}", value);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Separator();

            var figmaDocument = (FigmaDocument)target;
            if (figmaDocument.allLogs.Any())
            {
                isAllLogsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isAllLogsExpanded, "All Logs");
                if (isAllLogsExpanded)
                {
                    foreach (var logRef in figmaDocument.allLogs)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(GetLogIcon(logRef.log.type), 
                            GUILayout.Width(16), GUILayout.ExpandHeight(true));

                        if (GUILayout.Button(logRef.log.message,
                                new GUIStyle(EditorStyles.wordWrappedMiniLabel) { alignment = TextAnchor.MiddleLeft }))
                        {
                            if (logRef.target != null)
                            {
                                EditorGUIUtility.PingObject(logRef.target);
                            }
                        }

                        if (logRef.target != null)
                        {
                            EditorGUILayout.LabelField(EditorGUIUtility.IconContent("tab_next"), GUILayout.Width(16));
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}