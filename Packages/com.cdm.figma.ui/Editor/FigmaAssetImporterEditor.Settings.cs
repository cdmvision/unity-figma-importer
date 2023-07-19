using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    public partial class FigmaAssetImporterEditor
    {
        private readonly GUIContent[] _sampleCountContents =
        {
            new GUIContent("None"),
            new GUIContent("2 samples"),
            new GUIContent("4 samples"),
            new GUIContent("8 samples")
        };

        private readonly int[] _sampleCountValues =
        {
            1,
            2,
            4,
            8
        };

        private void DrawSettingsGui()
        {
            DrawBasicSettingsGui();
            EditorGUILayout.Space();

            DrawSpriteSettingsGui();
            EditorGUILayout.Space();

            DrawLocalizationConverterGui();
            EditorGUILayout.Space();

            DrawEffectConvertersGui();
        }

        private void DrawBasicSettingsGui()
        {
            EditorGUILayout.LabelField("Basic Settings", EditorStyles.boldLabel);
            _layer.intValue = EditorGUILayout.LayerField(_layer.displayName, _layer.intValue);
            EditorGUILayout.PropertyField(_markExternalAssetAsDependency);
            EditorGUILayout.PropertyField(_generateUniqueNodeName);
        }

        private void DrawSpriteSettingsGui()
        {
            EditorGUILayout.LabelField("Sprite Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_pixelsPerUnit);
            EditorGUILayout.PropertyField(_scaleFactor);
            EditorGUILayout.PropertyField(_gradientResolution);
            EditorGUILayout.PropertyField(_minTextureSize);
            EditorGUILayout.PropertyField(_maxTextureSize);
            EditorGUILayout.PropertyField(_wrapMode);
            EditorGUILayout.PropertyField(_filterMode);
            EditorGUILayout.PropertyField(_expandEdges);
            IntPopup(_sampleCount, _sampleCountContents, _sampleCountValues);

            if (_expandEdges.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(
                    $"{_expandEdges.displayName} might not work correctly on Unity Cloud Build.", MessageType.Warning);
            }
        }

        private void DrawLocalizationConverterGui()
        {
            EditorGUILayout.LabelField("Localization", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Converter");
            EditorGUILayout.PropertyField(_localizationConverter);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEffectConvertersGui()
        {
            EditorGUILayout.PropertyField(_effectConverters);
        }

        private static void IntPopup(SerializedProperty prop, GUIContent[] displayedOptions, int[] options)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
            
            var value = EditorGUILayout.IntPopup(
                new GUIContent(prop.displayName), prop.intValue, displayedOptions, options);
            
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.intValue = value;
            }
        }
    }
}