using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Styles.Properties
{
    [CustomPropertyDrawer(typeof(StylePropertyBase), true)]
    public class StylePropertyDrawer : PropertyDrawer
    {
        // Cached style to use to draw the popup button.
        private GUIStyle _popupStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const int gap = 5;

            if (_popupStyle == null)
            {
                _popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                _popupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var enabledRect = new Rect(position.x, position.y, 12, position.height);
            var enabledProperty = property.FindPropertyRelative("enabled");
            EditorGUI.PropertyField(enabledRect, enabledProperty, GUIContent.none);

            var guiEnabled = GUI.enabled;
            GUI.enabled = enabledProperty.boolValue;

            var offset = enabledRect.width + gap;

            // Draw label
            position.x = enabledRect.x + offset;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var valueProperty = property.FindPropertyRelative("_value");
            var valueRect = new Rect(position.x - offset, position.y, position.width, position.height);
            PropertyFieldWithChildren(valueRect, valueProperty, GUIContent.none);

            var endProperty = property.GetEndProperty();
            while (valueProperty.NextVisible(false))
            {
                if (SerializedProperty.EqualContents(valueProperty, endProperty))
                {
                    break;
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(valueProperty, false);
                EditorGUI.indentLevel--;
            }

            // Set gui activation back to what it was.
            GUI.enabled = guiEnabled;

            // Set indent back to what it was.
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private static void PropertyFieldWithChildren(Rect rect, SerializedProperty property, GUIContent content)
        {
            if (EditorGUI.PropertyField(rect, property, GUIContent.none))
            {
                var endProperty = property.GetEndProperty();
                while (property.NextVisible(true))
                {
                    if (SerializedProperty.EqualContents(property, endProperty))
                    {
                        break;
                    }

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(property, false);
                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}