using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    [CustomEditor(typeof(FigmaFileAsset))]
    public class FigmaFileAssetEditor : Editor
    {
        private const string VisualTreeFolder = "Packages/com.cdm.figma/Editor Default Resources";

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{VisualTreeFolder}/FigmaFileAsset.uxml");
            visualTree.CloneTree(root);

            var listItem = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{VisualTreeFolder}/FigmaFileAsset_PageListItem.uxml");
            
            var listView = root.Q<ListView>("pagesList");
            listView.makeItem = () => listItem.Instantiate();
            listView.bindItem = (e, i) =>
            {
                e.Q<Label>().text = ((FigmaFileAsset) target).pages[i].name;
            };
            
            root.Q<Button>("copyContentButton").clicked += () =>
            {
                var file = ((FigmaFileAsset) target);
                if (file.content != null)
                {
                    GUIUtility.systemCopyBuffer = file.content.text;   
                }
            };
            
            return root;
        }
    }
}