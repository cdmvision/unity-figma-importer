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
        private SerializedProperty _scaleFactor;
        private SerializedProperty _wrapMode;
        private SerializedProperty _filterMode;
        private SerializedProperty _sampleCount;
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
            new GUIContent("Bindings")
        };

        private const int PagesTabIndex = 0;
        private const int FontsTabIndex = 1;
        private const int SettingsTabIndex = 2;
        private const int BindingsTabIndex = 3;

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
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

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

            _selectedTabIndex = selectedTabIndex;
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Separator();
            ApplyRevertGUI();
        }
    }
}