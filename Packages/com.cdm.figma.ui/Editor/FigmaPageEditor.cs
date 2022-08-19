using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [CustomEditor(typeof(FigmaPage), editorForChildClasses: true)]
    public class FigmaPageEditor : FigmaNodeEditor
    {
        private bool isAllLogsExpanded
        {
            get => EditorPrefs.GetBool($"{nameof(FigmaPageEditor)}.{nameof(isAllLogsExpanded)}", false);
            set => EditorPrefs.SetBool($"{nameof(FigmaPageEditor)}.{nameof(isAllLogsExpanded)}", value);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Separator();
            var figmaPage = (FigmaPage)target;
            if (figmaPage.allLogs.Any())
            {
                isAllLogsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isAllLogsExpanded, "All Logs");
                if (isAllLogsExpanded)
                {
                    foreach (var logRef in figmaPage.allLogs)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(GetLogIcon(logRef.log.type), GUILayout.Width(16));

                        if (GUILayout.Button(logRef.log.message,
                                new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleLeft }))
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