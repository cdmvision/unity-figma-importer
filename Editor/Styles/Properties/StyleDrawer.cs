using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor.Styles.Properties
{
    namespace Cdm.Figma.UI.Editor.Styles.Properties
    {
        //[CustomPropertyDrawer(typeof(Style), true)]
        public class StylePropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                const int gap = 5;
                
                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(position, label, property);

                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                var enabledRect = new Rect(position.x, position.y, 12, position.height);
                var enabledProperty = property.FindPropertyRelative("_enabled");
                EditorGUI.PropertyField(enabledRect, enabledProperty, GUIContent.none);

                var guiEnabled = GUI.enabled;
                GUI.enabled = enabledProperty.boolValue;

                var offset = enabledRect.width + gap;

                // Draw label
                position.x = enabledRect.x + offset;
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                var selectorProperty = property.FindPropertyRelative("_selector");
                var selectorRect = new Rect(position.x - offset, position.y, position.width, position.height);
                EditorGUI.PropertyField(selectorRect, selectorProperty, GUIContent.none);
                //PropertyFieldWithChildren(valueRect, valueProperty, GUIContent.none);

                /*var endProperty = property.GetEndProperty();
                while (valueProperty.NextVisible(false))
                {
                    if (SerializedProperty.EqualContents(valueProperty, endProperty))
                    {
                        break;
                    }

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(valueProperty, false);
                    EditorGUI.indentLevel--;
                }*/

                // Set gui activation back to what it was.
                GUI.enabled = guiEnabled;

                // Set indent back to what it was.
                EditorGUI.indentLevel = indent;
                EditorGUI.EndProperty();
            }
        }
    }
}