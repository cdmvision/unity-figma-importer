using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    [CustomEditor(typeof(FigmaImporter))]
    public class FigmaImporterEditor : Editor
    {
        private const string VisualTreePath = "Packages/com.cdm.figma/Editor Default Resources/FigmaImporter.uxml";

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            visualTree.CloneTree(root);

            root.Q<Button>("accessTokenHelpButton").clicked += () =>
            {
                Application.OpenURL("https://www.figma.com/developers/api#access-tokens");
            };
            
            root.Q<Button>("downloadFilesButton").clicked += async () =>
            {
                await ((FigmaImporter) target).GetFilesAsync();
            };
            
            root.Q<Button>("generateViewsButton").clicked += () =>
            {
                Debug.Log("GENERATE VIEWS!!!!");
            };
            
            return root;
        }
    }
}