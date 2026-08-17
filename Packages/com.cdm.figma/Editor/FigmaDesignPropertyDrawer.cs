using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.Editor
{
    /// <summary>
    /// Gives <see cref="FigmaDesign"/> fields a working picker.
    /// </summary>
    /// <remarks>
    /// The importer registers the design's game object with the asset, and the components on it are
    /// not visible sub assets, so Unity's own picker finds nothing of this type and the only way to
    /// fill the field is to drag the asset onto it. The button added here lists the designs in the
    /// project instead. Dragging still works, because the object field is still an object field.
    /// </remarks>
    [CustomPropertyDrawer(typeof(FigmaDesign), useForChildren: true)]
    public class FigmaDesignPropertyDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 24f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var fieldRect = new Rect(position) { width = position.width - ButtonWidth };
            var buttonRect = new Rect(position) { xMin = fieldRect.xMax };

            var designType = GetDesignType();

            EditorGUI.BeginChangeCheck();
            var picked = EditorGUI.ObjectField(
                fieldRect, label, property.objectReferenceValue, designType, allowSceneObjects: false);
            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = picked;
            }

            if (EditorGUI.DropdownButton(buttonRect, new GUIContent("...", "Pick a Figma design"),
                    FocusType.Passive, EditorStyles.miniButton))
            {
                ShowMenu(property, designType);
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// The element type for a list or array field, the field type otherwise.
        /// </summary>
        private Type GetDesignType()
        {
            var type = fieldInfo.FieldType;

            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType && type.GetGenericArguments().Length == 1)
            {
                type = type.GetGenericArguments()[0];
            }

            return type != null && typeof(FigmaDesign).IsAssignableFrom(type) ? type : typeof(FigmaDesign);
        }

        private static void ShowMenu(SerializedProperty property, Type designType)
        {
            // A SerializedProperty is not safe to hold on to past this frame, so the menu items
            // re-fetch it from the serialized object by path when one of them is chosen.
            var serializedObject = property.serializedObject;
            var propertyPath = property.propertyPath;
            var current = property.objectReferenceValue;

            void Assign(UnityEngine.Object value)
            {
                serializedObject.Update();
                serializedObject.FindProperty(propertyPath).objectReferenceValue = value;
                serializedObject.ApplyModifiedProperties();
            }

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("None"), current == null, () => Assign(null));

            var designs = FindDesigns(designType);
            if (designs.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No design of this type in the project"));
            }
            else
            {
                menu.AddSeparator("");

                foreach (var (design, assetPath) in designs)
                {
                    var title = string.IsNullOrEmpty(design.title) ? design.name : design.title;
                    var entry = $"{Path.GetFileNameWithoutExtension(assetPath)} ({title})";

                    var captured = design;
                    menu.AddItem(new GUIContent(entry), ReferenceEquals(current, design),
                        () => Assign(captured));
                }
            }

            menu.ShowAsContext();
        }

        private static List<(FigmaDesign design, string assetPath)> FindDesigns(Type designType)
        {
            var extensions = GetDesignExtensions();
            var results = new List<(FigmaDesign, string)>();

            // Filtered by extension before anything is loaded: the alternative is loading every
            // GameObject asset in the project, prefabs included, to look for one component.
            foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
            {
                if (!assetPath.StartsWith("Assets/", StringComparison.Ordinal))
                    continue;

                if (!extensions.Contains(Path.GetExtension(assetPath)))
                    continue;

                var go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (go == null)
                    continue;

                if (go.GetComponent(designType) is FigmaDesign design)
                {
                    results.Add((design, assetPath));
                }
            }

            return results;
        }

        /// <summary>
        /// The default extension plus whatever the downloaders in this project are set to write.
        /// </summary>
        private static HashSet<string> GetDesignExtensions()
        {
            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".figma" };

            foreach (var guid in AssetDatabase.FindAssets($"t:{nameof(FigmaDownloaderAsset)}"))
            {
                var downloader =
                    AssetDatabase.LoadAssetAtPath<FigmaDownloaderAsset>(AssetDatabase.GUIDToAssetPath(guid));

                if (downloader != null && !string.IsNullOrEmpty(downloader.assetExtension))
                {
                    extensions.Add("." + downloader.assetExtension.TrimStart('.'));
                }
            }

            return extensions;
        }
    }
}
