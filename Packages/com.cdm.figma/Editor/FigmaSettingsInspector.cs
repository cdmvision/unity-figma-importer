using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    /// <summary>
    /// Custom editor for Figma Settings, displays the figma token
    /// </summary>
    [CustomEditor(typeof(FigmaSettings))]
    public class FigmaSettingsInspector : Editor
    {
        private VisualElement rootElement;
        private SerializedObject settings;
        public void OnEnable()
        {
            settings = new SerializedObject(target);
            rootElement = new VisualElement();
            rootElement.Bind(settings);

            Label figmaHeader = new Label("Settings");
            figmaHeader.AddToClassList("heading");
            rootElement.Add(figmaHeader);

            TextField figmaTokenField = new TextField("Figma Token");
            figmaTokenField.BindProperty(settings.FindProperty("FigmaToken"));
            rootElement.Add(figmaTokenField);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(FigmaSettings.FigmaStylesheetPath);
            rootElement.styleSheets.Add(styleSheet);

        }

        public override VisualElement CreateInspectorGUI()
        {
            return rootElement;

        }
    }
}