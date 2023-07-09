using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using Cdm.Figma.UI.Utils;
using TMPro;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [CustomEditor(typeof(FigmaAssetImporter), editorForChildClasses: true)]
    public class FigmaAssetImporterEditor : ScriptedImporterEditor
    {
        private SerializedProperty _pages;
        private SerializedProperty _pageReferences;
        private SerializedProperty _fallbackFont;
        private SerializedProperty _fonts;

        private SerializedProperty _layer;
        private SerializedProperty _pixelsPerUnit;
        private SerializedProperty _gradientResolution;
        private SerializedProperty _minTextureSize;
        private SerializedProperty _maxTextureSize;
        private SerializedProperty _scaleFactor;
        private SerializedProperty _wrapMode;
        private SerializedProperty _filterMode;
        private SerializedProperty _sampleCount;
        private SerializedProperty _effectConverters;
        private SerializedProperty _localizationConverter;
        private SerializedProperty _markExternalAssetAsDependency;
        private SerializedProperty _assetBindings;

        private int _selectedTabIndex = 0;
        private int _errorCount = 0;
        private int _warningCount = 0;
        
        private readonly Dictionary<Type, List<TypeBindingData>> _bindings = 
            new Dictionary<Type, List<TypeBindingData>>();

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

            _pages = serializedObject.FindProperty("_pages");
            _pageReferences = serializedObject.FindProperty("_pageReferences");
            _fonts = serializedObject.FindProperty("_fonts");
            _fallbackFont = serializedObject.FindProperty("_fallbackFont");

            _layer = serializedObject.FindProperty("_layer");
            _pixelsPerUnit = serializedObject.FindProperty("_pixelsPerUnit");
            _gradientResolution = serializedObject.FindProperty("_gradientResolution");
            _minTextureSize = serializedObject.FindProperty("_minTextureSize");
            _maxTextureSize = serializedObject.FindProperty("_maxTextureSize");
            _scaleFactor = serializedObject.FindProperty("_scaleFactor");
            _wrapMode = serializedObject.FindProperty("_wrapMode");
            _filterMode = serializedObject.FindProperty("_filterMode");
            _sampleCount = serializedObject.FindProperty("_sampleCount");
            _effectConverters = serializedObject.FindProperty("_effectConverters");
            _localizationConverter = serializedObject.FindProperty("_localizationConverter");
            _markExternalAssetAsDependency = serializedObject.FindProperty("_markExternalAssetAsDependency");
            _assetBindings = serializedObject.FindProperty("_assetBindings");

            var importer = (FigmaAssetImporter)target;
            var figmaDesign = AssetDatabase.LoadAssetAtPath<FigmaDesign>(importer.assetPath);
            if (figmaDesign != null)
            {
                foreach (var logReference in figmaDesign.document.GetLogs())
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

            var selectedTabIndex = GUILayout.Toolbar(_selectedTabIndex, new GUIContent[]
            {
                new GUIContent("Pages"), 
                new GUIContent("Fonts"), 
                new GUIContent("Settings"), 
                new GUIContent("Bindings")
            });

            EditorGUILayout.Space();

            if (selectedTabIndex == 0)
            {
                DrawPagesGui();
            }
            else if (selectedTabIndex == 1)
            {
                DrawFontsGui();
            }
            else if (selectedTabIndex == 2)
            {
                DrawSettingsGui();
            }
            else if (selectedTabIndex == 3)
            {
                DrawBindingsGui(selectedTabIndex != _selectedTabIndex);
            }

            _selectedTabIndex = selectedTabIndex;
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

        private void DrawFontsGui()
        {
            EditorGUILayout.PropertyField(_fallbackFont);
            EditorGUILayout.Space();

            for (var i = 0; i < _fonts.arraySize; i++)
            {
                var element = _fonts.GetArrayElementAtIndex(i);
                var nameProperty = element.FindPropertyRelative("fontName");
                var fontProperty = element.FindPropertyRelative("font");

                EditorGUILayout.PropertyField(fontProperty, new GUIContent(nameProperty.stringValue));
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Search and Remap"))
                {
                    SearchForFonts();
                }
            }
        }

        private struct TypeBindingData
        {
            public MemberInfo memberInfo;
            public FigmaAssetAttribute attribute;

            public TypeBindingData(MemberInfo memberInfo, FigmaAssetAttribute attribute)
            {
                this.memberInfo = memberInfo;
                this.attribute = attribute;
            }
        }

        private void RefreshAssetBindings()
        {
            _bindings.Clear();
            
            var monoBehaviours = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => typeof(MonoBehaviour).IsAssignableFrom(t)));
            
            foreach (var monoBehaviour in monoBehaviours)
            {
                var members = monoBehaviour.GetMembers(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var member in members)
                {
                    if (!member.MemberType.HasFlag(MemberTypes.Field) &&
                        !member.MemberType.HasFlag(MemberTypes.Property))
                        continue;

                    var figmaAssetAttribute = (FigmaAssetAttribute)member.GetCustomAttributes()
                        .FirstOrDefault(x => x.GetType() == typeof(FigmaAssetAttribute));

                    if (figmaAssetAttribute == null)
                        continue;

                    if (!_bindings.ContainsKey(monoBehaviour))
                    {
                        _bindings.Add(monoBehaviour, new List<TypeBindingData>());
                    }
                    
                    _bindings[monoBehaviour].Add(new TypeBindingData(member, figmaAssetAttribute));
                }
            }
        }
        
        private void DrawBindingsGui(bool refresh)
        {
            if (refresh)
            {
                RefreshAssetBindings();
            }

            foreach (var binding in _bindings)
            {
                var type = binding.Key;
                var typeIndex = FindAssetBindingByType(type);
                
                EditorGUILayout.LabelField(type.FullName, EditorStyles.boldLabel);
                EditorGUI.indentLevel += 1;
                
                foreach (var data in binding.Value)
                {
                    var memberIndex = -1;
                    var member = data.memberInfo;
                    var attribute = data.attribute;
                    UnityEngine.Object value = null;
                    
                    if (typeIndex >= 0)
                    {
                       memberIndex = FindAssetBindingMemberByType(typeIndex, member);
                       if (memberIndex >= 0)
                       {
                           var bindingElement = _assetBindings.GetArrayElementAtIndex(typeIndex);
                           var members = bindingElement.FindPropertyRelative("bindings");
                           var memberElement = members.GetArrayElementAtIndex(memberIndex);
                           var assetProperty = memberElement.FindPropertyRelative("asset");
                           value = assetProperty.objectReferenceValue;
                       }
                    }
                    
                    var fieldType = ReflectionHelper.GetUnderlyingType(member);
                    var displayName = !string.IsNullOrEmpty(attribute.name) 
                        ? attribute.name 
                        : ObjectNames.NicifyVariableName(member.Name);
                    
                    EditorGUI.BeginChangeCheck();
                    var newValue = 
                        EditorGUILayout.ObjectField(new GUIContent(displayName, member.Name), value, fieldType, false);
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (typeIndex < 0)
                        {
                            _assetBindings.arraySize += 1;
                            typeIndex = _assetBindings.arraySize - 1;
                        }

                        var bindingElement = _assetBindings.GetArrayElementAtIndex(typeIndex);
                        bindingElement.FindPropertyRelative("type").stringValue = type.AssemblyQualifiedName;
                        
                        var members = bindingElement.FindPropertyRelative("bindings");
                        
                        if (memberIndex < 0)
                        {
                            members.arraySize += 1;
                            memberIndex = members.arraySize - 1;
                        }
                        
                        var memberElement = members.GetArrayElementAtIndex(memberIndex);
                        var nameProperty = memberElement.FindPropertyRelative("name");
                        var assetProperty = memberElement.FindPropertyRelative("asset");

                        nameProperty.stringValue = member.Name;
                        assetProperty.objectReferenceValue = newValue;
                    }
                }
                
                EditorGUI.indentLevel -= 1;
                EditorGUILayout.Space();
            }
        }

        private int FindAssetBindingByType(Type targetType)
        {
            if (targetType == null)
                return -1;
            
            for (var i = 0; i < _assetBindings.arraySize; i++)
            {
                var binding = _assetBindings.GetArrayElementAtIndex(i);
                
                var type = Type.GetType(binding.FindPropertyRelative("type")?.stringValue ?? "");
                if (type == targetType)
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindAssetBindingMemberByType(int bindingIndex, MemberInfo memberInfo)
        {
            var binding = _assetBindings.GetArrayElementAtIndex(bindingIndex);
            var members = binding.FindPropertyRelative("bindings");

            for (var i = 0; i < members.arraySize; i++)
            {
                var member = members.GetArrayElementAtIndex(i);
                var memberName = member.FindPropertyRelative("name");

                if (memberName.stringValue.Equals(memberInfo.Name))
                {
                    return i;
                }
            }

            return -1;
        }
        
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
            IntPopup(_sampleCount, _sampleCountContents, _sampleCountValues);
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

        private void SearchForFonts()
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
    }
}