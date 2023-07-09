using System;
using System.Collections.Generic;
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
        private const string AssetBindingTypePropertyPath =
            nameof(FigmaAssetImporter.SerializableAssetBinding.type);

        private const string AssetBindingsPropertyPath =
            nameof(FigmaAssetImporter.SerializableAssetBinding.bindings);

        private const string AssetBindingNamePropertyPath =
            nameof(FigmaAssetImporter.SerializableAssetBindingMember.name);

        private const string AssetBindingAssetPropertyPath =
            nameof(FigmaAssetImporter.SerializableAssetBindingMember.asset);

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

        private readonly Dictionary<Type, List<AssetBindingMember>> _bindings =
            new Dictionary<Type, List<AssetBindingMember>>();

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
                        _bindings.Add(monoBehaviour, new List<AssetBindingMember>());
                    }

                    _bindings[monoBehaviour].Add(new AssetBindingMember(member, figmaAssetAttribute));
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
                var bindingType = binding.Key;

                EditorGUILayout.LabelField(bindingType.FullName, EditorStyles.boldLabel);
                EditorGUI.indentLevel += 1;

                foreach (var memberBinding in binding.Value)
                {
                    var bindingIndex = -1;
                    var member = memberBinding.member;
                    var attribute = memberBinding.attribute;
                    UnityEngine.Object value = null;

                    var bindingTypeIndex = FindSerializableAssetBindingByType(bindingType);
                    if (bindingTypeIndex >= 0)
                    {
                        bindingIndex = FindSerializableAssetBindingMemberByType(bindingTypeIndex, member);

                        if (bindingIndex >= 0)
                        {
                            var bindingProperty = _assetBindings.GetArrayElementAtIndex(bindingTypeIndex);
                            var bindingsProperty = bindingProperty.FindPropertyRelative(AssetBindingsPropertyPath);
                            var memberProperty = bindingsProperty.GetArrayElementAtIndex(bindingIndex);
                            var assetProperty = memberProperty.FindPropertyRelative(AssetBindingAssetPropertyPath);
                            value = assetProperty.objectReferenceValue;
                        }
                    }

                    var displayName = !string.IsNullOrEmpty(attribute.name)
                        ? attribute.name
                        : ObjectNames.NicifyVariableName(member.Name);

                    var fieldType = ReflectionHelper.GetUnderlyingType(member);

                    EditorGUI.BeginChangeCheck();
                    var newValue = EditorGUILayout.ObjectField(
                        new GUIContent(displayName, member.Name), value, fieldType, false);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (newValue != null)
                        {
                            if (bindingTypeIndex < 0)
                            {
                                _assetBindings.arraySize += 1;
                                bindingTypeIndex = _assetBindings.arraySize - 1;
                            }

                            var bindingProperty = _assetBindings.GetArrayElementAtIndex(bindingTypeIndex);
                            var bindingTypeProperty = bindingProperty.FindPropertyRelative(AssetBindingTypePropertyPath);
                            bindingTypeProperty.stringValue = bindingType.AssemblyQualifiedName;

                            var bindingsProperty = bindingProperty.FindPropertyRelative(AssetBindingsPropertyPath);

                            if (bindingIndex < 0)
                            {
                                bindingsProperty.arraySize += 1;
                                bindingIndex = bindingsProperty.arraySize - 1;
                            }

                            var memberProperty = bindingsProperty.GetArrayElementAtIndex(bindingIndex);
                            var nameProperty = memberProperty.FindPropertyRelative(AssetBindingNamePropertyPath);
                            var assetProperty = memberProperty.FindPropertyRelative(AssetBindingAssetPropertyPath);

                            nameProperty.stringValue = member.Name;
                            assetProperty.objectReferenceValue = newValue;
                        }
                        else
                        {
                            if (bindingTypeIndex >= 0)
                            {
                                var bindingProperty = _assetBindings.GetArrayElementAtIndex(bindingTypeIndex);
                                var bindingsProperty = bindingProperty.FindPropertyRelative(AssetBindingsPropertyPath);
                                
                                if (bindingIndex >= 0)
                                {
                                    bindingsProperty.DeleteArrayElementAtIndex(bindingIndex);
                                }

                                if (bindingsProperty.arraySize == 0)
                                {
                                    _assetBindings.DeleteArrayElementAtIndex(bindingTypeIndex);
                                }
                            }
                        }
                    }
                }

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.Space();
            }
        }

        private int FindSerializableAssetBindingByType(Type targetType)
        {
            if (targetType == null)
                return -1;

            for (var i = 0; i < _assetBindings.arraySize; i++)
            {
                var binding = _assetBindings.GetArrayElementAtIndex(i);
                var bindingTypeName = binding.FindPropertyRelative(AssetBindingTypePropertyPath)?.stringValue;
                
                var type = Type.GetType(bindingTypeName ?? "");
                if (type == targetType)
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindSerializableAssetBindingMemberByType(int bindingIndex, MemberInfo memberInfo)
        {
            const string bindingsPropertyPath = nameof(FigmaAssetImporter.SerializableAssetBinding.bindings);
            const string namePropertyPath = nameof(FigmaAssetImporter.SerializableAssetBindingMember.name);

            var binding = _assetBindings.GetArrayElementAtIndex(bindingIndex);
            var members = binding.FindPropertyRelative(bindingsPropertyPath);

            for (var i = 0; i < members.arraySize; i++)
            {
                var member = members.GetArrayElementAtIndex(i);
                var memberName = member.FindPropertyRelative(namePropertyPath);

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

        private struct AssetBindingMember
        {
            public MemberInfo member;
            public FigmaAssetAttribute attribute;

            public AssetBindingMember(MemberInfo member, FigmaAssetAttribute attribute)
            {
                this.member = member;
                this.attribute = attribute;
            }
        }
    }
}