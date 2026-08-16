using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    public partial class FigmaAssetImporterEditor
    {
        private void DrawPagesGui()
        {
            DrawWarningsGui();

            EditorGUILayout.Separator();
            for (var i = 0; i < _pages.arraySize; i++)
            {
                var pageProperty = _pages.GetArrayElementAtIndex(i);
                var enabledProperty = pageProperty.FindPropertyRelative("enabled");
                var nameProperty = pageProperty.FindPropertyRelative("name");
                var pageRef = _pageReferences.GetArrayElementAtIndex(i);

                using (new EditorGUILayout.HorizontalScope())
                {
                    enabledProperty.boolValue =
                        EditorGUILayout.ToggleLeft(GUIContent.none, enabledProperty.boolValue, GUILayout.Width(16));

                    if (enabledProperty.boolValue)
                    {
                        var page = pageRef.objectReferenceValue as FigmaPage;
                        var label = EditorGUIUtility.ObjectContent(pageRef.objectReferenceValue, typeof(FigmaPage));

                        var style = new GUIStyle(EditorStyles.objectField);
                        style.fixedHeight = EditorGUIUtility.singleLineHeight;
                        style.imagePosition = page != null ? ImagePosition.ImageLeft : ImagePosition.TextOnly;

                        EditorGUILayout.PrefixLabel(nameProperty.stringValue);
                        if (GUILayout.Button(label, style))
                        {
                            EditorGUIUtility.PingObject(page != null ? page.gameObject : null);
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField(new GUIContent(nameProperty.stringValue));
                    }
                }
            }
        }

        private void DrawWarningsGui()
        {
            var message = "";
            if (_errorCount > 0 && _warningCount > 0)
            {
                message = $"Asset imported with {_errorCount} errors and {_warningCount} warnings.";
            }
            else if (_errorCount > 0)
            {
                message = $"Asset imported with {_errorCount} errors.";
            }
            else if (_warningCount > 0)
            {
                message = $"Asset imported with {_warningCount} warnings.";
            }

            if (_errorCount > 0 || _warningCount > 0)
            {
                EditorGUILayout.HelpBox(message, MessageType.Warning, true);
            }
        }
    }
}