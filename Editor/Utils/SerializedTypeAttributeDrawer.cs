using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor.Utils
{
    [CustomPropertyDrawer(typeof(SerializedTypeAttribute))]
    public class SerializedTypeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            var underlyingType = Type.GetType(property.stringValue);
            var valueContent = underlyingType != null
                ? new GUIContent(underlyingType.Name, underlyingType.AssemblyQualifiedName)
                : new GUIContent("None");

            if (EditorGUI.DropdownButton(position, valueContent, FocusType.Keyboard))
            {
                var menu = new GenericMenu();
                var serializedTypeAttribute = (SerializedTypeAttribute)attribute;
                var baseType = Type.GetType(serializedTypeAttribute.baseType);

                var types = baseType != null
                    ? AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes().Where(t => baseType.IsAssignableFrom(t)))
                        .Where(t => !t.IsAbstract).ToArray()
                    : Array.Empty<Type>();

                menu.AddItem(new GUIContent("None"), underlyingType == null,
                    _ => { UpdatePropertyValue(property, ""); }, "");
                menu.AddSeparator("");

                foreach (var type in types)
                {
                    menu.AddItem(new GUIContent(type.Name, type.AssemblyQualifiedName),
                        type.AssemblyQualifiedName == property.stringValue,
                        data => { UpdatePropertyValue(property, (string)data); },
                        type.AssemblyQualifiedName);
                }

                menu.DropDown(position);
            }

            EditorGUI.EndProperty();
        }

        private static void UpdatePropertyValue(SerializedProperty property, string value)
        {
            property.serializedObject.Update();
            property.stringValue = value;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}