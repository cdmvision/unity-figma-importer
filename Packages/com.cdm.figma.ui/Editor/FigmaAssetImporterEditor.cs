using System;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [CustomEditor(typeof(FigmaAssetImporter), editorForChildClasses: true)]
    public partial class FigmaAssetImporterEditor : ScriptedImporterEditor
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
        private SerializedProperty _rectTextureSize;
        private SerializedProperty _scaleFactor;
        private SerializedProperty _wrapMode;
        private SerializedProperty _filterMode;
        private SerializedProperty _sampleCount;
        private SerializedProperty _expandEdges;
        
        private SerializedProperty _effectConverters;
        private SerializedProperty _localizationConverter;
        private SerializedProperty _markExternalAssetAsDependency;
        private SerializedProperty _generateUniqueNodeName;
        private SerializedProperty _assetBindings;

        private int _selectedTabIndex = 0;
        private int _errorCount = 0;
        private int _warningCount = 0;
        
        private readonly GUIContent[] _tabs =
        {
            new GUIContent("Pages"),
            new GUIContent("Fonts"),
            new GUIContent("Settings"),
            new GUIContent("Bindings"),
            new GUIContent("Stats")
        };

        private const int PagesTabIndex = 0;
        private const int FontsTabIndex = 1;
        private const int SettingsTabIndex = 2;
        private const int BindingsTabIndex = 3;
        private const int StatsTabIndex = 4;

        public override bool showImportedObject => false;
        protected override bool useAssetDrawPreview => false;
        
        private FigmaDesign _figmaDesign;
        
        public override void OnEnable()
        {
            base.OnEnable();

            var path = AssetDatabase.GetAssetPath(target);
            
            _figmaDesign = AssetDatabase.LoadAssetAtPath<FigmaDesign>(path);
            
            _pages = serializedObject.FindProperty("_pages");
            _pageReferences = serializedObject.FindProperty("_pageReferences");
            _fonts = serializedObject.FindProperty("_fonts");
            _fallbackFont = serializedObject.FindProperty("_fallbackFont");

            _layer = serializedObject.FindProperty("_layer");
            _pixelsPerUnit = serializedObject.FindProperty("_pixelsPerUnit");
            _gradientResolution = serializedObject.FindProperty("_gradientResolution");
            _minTextureSize = serializedObject.FindProperty("_minTextureSize");
            _maxTextureSize = serializedObject.FindProperty("_maxTextureSize");
            _rectTextureSize = serializedObject.FindProperty("_rectTextureSize");
            _scaleFactor = serializedObject.FindProperty("_scaleFactor");
            _wrapMode = serializedObject.FindProperty("_wrapMode");
            _filterMode = serializedObject.FindProperty("_filterMode");
            _sampleCount = serializedObject.FindProperty("_sampleCount");
            _expandEdges = serializedObject.FindProperty("_expandEdges");
            
            _effectConverters = serializedObject.FindProperty("_effectConverters");
            _localizationConverter = serializedObject.FindProperty("_localizationConverter");
            _markExternalAssetAsDependency = serializedObject.FindProperty("_markExternalAssetAsDependency");
            _generateUniqueNodeName = serializedObject.FindProperty("_generateUniqueNodeName");
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

            InitStats();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (_figmaDesign != null)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField($"Figma Design", EditorStyles.boldLabel);
                EditorGUILayout.Separator();
                
                LabelField("ID", _figmaDesign.id);
                LabelField("Title", _figmaDesign.title);
                LabelField("Version", _figmaDesign.version);
                LabelField("Last Modified", DateTime.Parse(_figmaDesign.lastModified).ToString("F"));    
            }
            
            EditorGUILayout.Space(12f, true);
            var selectedTabIndex = GUILayout.Toolbar(_selectedTabIndex, _tabs);
            EditorGUILayout.Space();

            if (selectedTabIndex == PagesTabIndex)
            {
                DrawPagesGui();
            }
            else if (selectedTabIndex == FontsTabIndex)
            {
                DrawFontsGui();
            }
            else if (selectedTabIndex == SettingsTabIndex)
            {
                DrawSettingsGui();
            }
            else if (selectedTabIndex == BindingsTabIndex)
            {
                DrawBindingsGui(selectedTabIndex != _selectedTabIndex);
            }
            else if (selectedTabIndex == StatsTabIndex)
            {
                DrawStatsGui();
            }

            _selectedTabIndex = selectedTabIndex;
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Separator();
            ApplyRevertGUI();
        }
        
        public override bool HasPreviewGUI()
        {
            return _figmaDesign != null && _figmaDesign.thumbnail != null;
        }

        public override void OnPreviewGUI(Rect previewArea, GUIStyle background)
        {
            base.OnPreviewGUI(previewArea, background);

            var thumbnail = _figmaDesign.thumbnail;

            var widthRatio = (previewArea.width - 8f) / thumbnail.width;
            var heightRatio = (previewArea.height - 8f) / thumbnail.height;
            var ratio = widthRatio > heightRatio ? heightRatio : widthRatio;

            var newWidth = thumbnail.width * ratio;
            var newHeight = thumbnail.height * ratio;
            var newX = previewArea.x + (previewArea.width - newWidth) * 0.5f;
            var newY = previewArea.y + (previewArea.height - newHeight) * 0.5f;

            var newPreviewArea = new Rect(newX, newY, newWidth, newHeight);
            GUI.DrawTexture(newPreviewArea, thumbnail);

            const float labelHeight = 40;
            EditorGUI.DropShadowLabel(
                new Rect(previewArea.x, previewArea.height - labelHeight - 6, previewArea.width, labelHeight),
                $"{_figmaDesign.name} - {DateTime.Parse(_figmaDesign.lastModified):G}");
        }
        
        private static void LabelField(string label, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(label);
                EditorGUILayout.SelectableLabel(value, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField(label, value);
            }
        }
    }
}