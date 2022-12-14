using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [CustomEditor(typeof(FigmaAssetImporter), editorForChildClasses: true)]
    public class FigmaAssetImporterEditor : ScriptedImporterEditor
    {
        private SerializedProperty _pagesProperty;
        private SerializedProperty _fallbackFontProperty;
        private SerializedProperty _fontsProperty;

        private SerializedProperty _pixelsPerUnit;
        private SerializedProperty _gradientResolution;
        private SerializedProperty _textureSize;
        private SerializedProperty _wrapMode;
        private SerializedProperty _filterMode;
        private SerializedProperty _sampleCount;

        private int _selectedTabIndex = 0;
        private int _errorCount = 0;
        private int _warningCount = 0;
        
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

            _pagesProperty = serializedObject.FindProperty("_pages");
            _fontsProperty = serializedObject.FindProperty("_fonts");
            _fallbackFontProperty = serializedObject.FindProperty("_fallbackFont");

            _pixelsPerUnit = serializedObject.FindProperty("_pixelsPerUnit");
            _gradientResolution = serializedObject.FindProperty("_gradientResolution");
            _textureSize = serializedObject.FindProperty("_textureSize");
            _wrapMode = serializedObject.FindProperty("_wrapMode");
            _filterMode = serializedObject.FindProperty("_filterMode");
            _sampleCount = serializedObject.FindProperty("_sampleCount");
            
            var importer = (FigmaAssetImporter)target;
            var figmaDesign = AssetDatabase.LoadAssetAtPath<FigmaDesign>(importer.assetPath);
            if (figmaDesign != null)
            {
                foreach (var logReference in figmaDesign.document.allLogs)
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

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _selectedTabIndex = GUILayout.Toolbar(_selectedTabIndex, new GUIContent[]
            {
                new GUIContent("Pages"), new GUIContent("Fonts"), new GUIContent("Settings")
            });

            EditorGUILayout.Space();
            
            if (_selectedTabIndex == 0)
            {
                DrawPagesGui();
            }
            else if (_selectedTabIndex == 1)
            {
                DrawFontsGui();
            }
            else if (_selectedTabIndex == 2)
            {
                DrawSettingsGui();
            }
            
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Separator();
            ApplyRevertGUI();
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

        private void DrawPagesGui()
        {
            DrawWarningsGui();
            
            EditorGUILayout.Separator();
            for (var i = 0; i < _pagesProperty.arraySize; i++)
            {
                var element = _pagesProperty.GetArrayElementAtIndex(i);
                
                var enabledProperty = element.FindPropertyRelative("enabled");
                var nameProperty = element.FindPropertyRelative("name");
                enabledProperty.boolValue = 
                    EditorGUILayout.ToggleLeft(new GUIContent(nameProperty.stringValue), enabledProperty.boolValue);
            }
            
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Extract Pages...", EditorStyles.miniButton, GUILayout.Width(196)))
            {
                ExtractPages();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawFontsGui()
        {
            EditorGUILayout.PropertyField(_fallbackFontProperty);
            EditorGUILayout.Space();
            
            for (var i = 0; i < _fontsProperty.arraySize; i++)
            {
                var element = _fontsProperty.GetArrayElementAtIndex(i);
                var nameProperty = element.FindPropertyRelative("fontName");
                var fontProperty = element.FindPropertyRelative("font");
                
                EditorGUILayout.PropertyField(fontProperty, new GUIContent(nameProperty.stringValue));
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Search & Add Fonts", EditorStyles.miniButton, GUILayout.Width(196)))
            {
                SearchAndAddFonts();
            }

            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawSettingsGui()
        {
            EditorGUILayout.LabelField("Sprite Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_pixelsPerUnit);
            EditorGUILayout.PropertyField(_gradientResolution);
            EditorGUILayout.PropertyField(_textureSize);
            EditorGUILayout.PropertyField(_wrapMode);
            EditorGUILayout.PropertyField(_filterMode);
            IntPopup(_sampleCount, _sampleCountContents, _sampleCountValues);
        }

        private void ExtractPages()
        {
            // use the first target for selecting the destination folder, but apply that path for all targets
            var importer = (AssetImporter)target;
            var destinationPath = importer.assetPath;
            destinationPath = EditorUtility.SaveFolderPanel(
                "Select Materials Folder", Path.GetDirectoryName(destinationPath), "");
            
            if (string.IsNullOrEmpty(destinationPath))
                return;
            
            destinationPath = FileUtil.GetProjectRelativePath(destinationPath);
            if (string.IsNullOrEmpty(destinationPath))
                return;
            
            try
            {
                // batch the extraction of the textures
                AssetDatabase.StartAssetEditing();
                
                var figmaDesign = AssetDatabase.LoadAssetAtPath<FigmaDesign>(importer.assetPath);
                //foreach (var page in figmaDesign.document)
                //{
                    var newAssetPath = Path.Combine(destinationPath, $"{figmaDesign.document.name}.prefab");
                    newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);

                    var pageIns = Instantiate(figmaDesign.document.gameObject);
                    var pageInstance = PrefabUtility.SaveAsPrefabAsset(pageIns, newAssetPath);
                    DestroyImmediate(pageIns);
                
                    //var assetImporter = AssetImporter.GetAtPath(subAssetPath);
                    importer.AddRemap(new AssetImporter.SourceAssetIdentifier(figmaDesign.document.gameObject), pageInstance);
                    
                    //var error = AssetDatabase.ExtractAsset(page.gameObject, newAssetPath);
                    //Debug.LogError(error);
                //}
                
                AssetDatabase.WriteImportSettingsIfDirty(importer.assetPath);
                AssetDatabase.ImportAsset(importer.assetPath, ImportAssetOptions.ForceUpdate);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
            
           /* var importer = (FigmaAssetImporter)target;
            var figmaDesign = AssetDatabase.LoadAssetAtPath<FigmaDesign>(importer.assetPath);
            if (figmaDesign == null)
                return;
            
            var assetPath = importer.assetPath;
            var path = EditorUtility.OpenFolderPanel("Select folder", Path.GetDirectoryName(assetPath), "");
            if (string.IsNullOrWhiteSpace(path))
                return;
            path = Path.Combine("Assets/", Path.GetRelativePath(Application.dataPath, path));
            
            var subAssetPath = AssetDatabase.GetAssetPath(figmaDesign.document.gameObject);

            var pageIns = Instantiate(figmaDesign.document.gameObject);
            var pageInstance = PrefabUtility.SaveAsPrefabAsset(pageIns, Path.Combine(path, $"{figmaDesign.document.name}.prefab"));
            DestroyImmediate(pageIns);
                
            var assetImporter = AssetImporter.GetAtPath(subAssetPath);
            assetImporter.AddRemap(new AssetImporter.SourceAssetIdentifier(figmaDesign.document.gameObject), pageInstance);
                
            AssetDatabase.WriteImportSettingsIfDirty(subAssetPath);
            AssetDatabase.ImportAsset(subAssetPath, ImportAssetOptions.ForceUpdate);*/
            
            /*foreach (var page in figmaDesign.document)
            {
                var subAssetPath = AssetDatabase.GetAssetPath(page.gameObject);

                var pageIns = Instantiate(page.gameObject);
                var pageInstance = PrefabUtility.SaveAsPrefabAsset(pageIns, Path.Combine(path, $"{page.name}.prefab"));
                DestroyImmediate(pageIns);
                
                var assetImporter = AssetImporter.GetAtPath(subAssetPath);
                assetImporter.AddRemap(new AssetImporter.SourceAssetIdentifier(page.gameObject), pageInstance);
                
                AssetDatabase.WriteImportSettingsIfDirty(subAssetPath);
                AssetDatabase.ImportAsset(subAssetPath, ImportAssetOptions.ForceUpdate);
            }*/
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

                for (var i = 0; i < _fontsProperty.arraySize; i++)
                {
                    var fontProperty = _fontsProperty.GetArrayElementAtIndex(i);

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
    }
}