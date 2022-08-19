using System.IO;
using System.Linq;
using Cdm.Figma.Editor;
using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [CustomEditor(typeof(FigmaAssetImporter), editorForChildClasses: true)]
    public class FigmaAssetImporterEditor : FigmaAssetImporterBaseEditor
    {
        private SerializedProperty _fallbackFont;
        private SerializedProperty _fonts;
        private ReorderableList _fontsList;

        private SerializedProperty _pixelsPerUnit;
        private SerializedProperty _gradientResolution;
        private SerializedProperty _textureSize;
        private SerializedProperty _wrapMode;
        private SerializedProperty _filterMode;
        private SerializedProperty _sampleCount;
        
        private int _errorCount = 0;
        private int _warningCount = 0;

        private bool isSpriteSettingsExpanded
        {
            get => EditorPrefs.GetBool($"{nameof(FigmaAssetImporterEditor)}.{nameof(isSpriteSettingsExpanded)}", false);
            set => EditorPrefs.SetBool($"{nameof(FigmaAssetImporterEditor)}.{nameof(isSpriteSettingsExpanded)}", value);
        }
        
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
        
        public override void OnEnable()
        {
            base.OnEnable();

            _fallbackFont = serializedObject.FindProperty("_fallbackFont");
            _fonts = serializedObject.FindProperty("_fonts");

            _pixelsPerUnit = serializedObject.FindProperty("_pixelsPerUnit");
            _gradientResolution = serializedObject.FindProperty("_gradientResolution");
            _textureSize = serializedObject.FindProperty("_textureSize");
            _wrapMode = serializedObject.FindProperty("_wrapMode");
            _filterMode = serializedObject.FindProperty("_filterMode");
            _sampleCount = serializedObject.FindProperty("_sampleCount");

            _fontsList = new ReorderableList(serializedObject, _fonts, false, true, false, false);
            _fontsList.drawHeaderCallback = DrawHeader;
            _fontsList.drawElementCallback = DrawElement;

            var importer = (FigmaAssetImporter)target;
            var figmaDesign = AssetDatabase.LoadAssetAtPath<FigmaDesign>(importer.assetPath);
            if (figmaDesign != null)
            {
                foreach (var page in figmaDesign.pages)
                {
                    foreach (var logReference in page.allLogs)
                    {
                        if (logReference.log.type == FigmaImporterLogType.Error)
                        {
                            _errorCount++;
                        }
                        else if (logReference.log.type == FigmaImporterLogType.Warning)
                        {
                            _warningCount++;
                        }
                    }
                }
            }
        }

        protected override void DrawGUI()
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

            base.DrawGUI();

            _fontsList.DoLayoutList();
            EditorGUILayout.PropertyField(_fallbackFont);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Search & Add Fonts", EditorStyles.miniButton, GUILayout.Width(196)))
            {
                SearchAndAddFonts();
            }

            EditorGUILayout.EndHorizontal();

            isSpriteSettingsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isSpriteSettingsExpanded, "Sprite Settings");
            if (isSpriteSettingsExpanded)
            {
                EditorGUILayout.PropertyField(_pixelsPerUnit);
                EditorGUILayout.PropertyField(_gradientResolution);
                EditorGUILayout.PropertyField(_textureSize);
                EditorGUILayout.PropertyField(_wrapMode);
                EditorGUILayout.PropertyField(_filterMode);    
                
                IntPopup(_sampleCount, _sampleCountContents, _sampleCountValues);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        private static void IntPopup(SerializedProperty prop, GUIContent[] displayedOptions, 
            int[] options)
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

        private void SearchAndAddFonts()
        {
            var path = EditorUtility.OpenFolderPanel("Select folder to be searched", "", "Assets");
            if (!string.IsNullOrWhiteSpace(path))
            {
                path = path.Replace("\\", "/").Replace(Application.dataPath.Replace("\\", "/"), "");
                if (path.Length > 0 && path[0] == '/')
                {
                    path = path.Substring(1, path.Length - 1);
                }

                path = Path.Combine("Assets", path);

                var guids = AssetDatabase.FindAssets($"t:{typeof(TMP_FontAsset)}", new[] { path });
                var assets = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

                for (var i = 0; i < _fonts.arraySize; i++)
                {
                    var fontProperty = _fonts.GetArrayElementAtIndex(i);

                    var fontNameProperty = fontProperty.FindPropertyRelative("fontName");
                    var fontAssetProperty = fontProperty.FindPropertyRelative("font");

                    if (fontAssetProperty.objectReferenceValue == null)
                    {
                        if (TryGetFontAsset(assets, fontNameProperty.stringValue, out var fontAsset))
                        {
                            fontAssetProperty.objectReferenceValue = fontAsset;

                            Debug.Log(
                                $"{fontNameProperty.stringValue} was mapped to {AssetDatabase.GetAssetPath(fontAsset)}");
                            EditorUtility.SetDirty(target);
                        }
                    }
                }

                AssetDatabase.SaveAssetIfDirty(target);
            }
        }

        private static bool TryGetFontAsset(string[] assets, string fontName, out TMP_FontAsset fontAsset)
        {
            foreach (var assetPath in assets)
            {
                var assetName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
                var tokens = fontName.Split("-");

                if (tokens.All(t => assetName.Contains(t.ToLower())))
                {
                    fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
                    return fontAsset != null;
                }
            }

            fontAsset = null;
            return false;
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _fonts.displayName);
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _fontsList.serializedProperty.GetArrayElementAtIndex(index);
            var nameProperty = element.FindPropertyRelative("fontName");
            var fontProperty = element.FindPropertyRelative("font");

            rect = new Rect(rect.x, rect.y + 2, rect.width, rect.height - 4);
            EditorGUI.PropertyField(rect, fontProperty, new GUIContent(nameProperty.stringValue));
        }
    }
}