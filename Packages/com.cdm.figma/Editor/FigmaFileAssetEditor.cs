using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    [CustomEditor(typeof(FigmaFileAsset))]
    public class FigmaFileAssetEditor : Editor
    { 
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PackageUtils.VisualTreeFolder}/FigmaFileAsset.uxml");
            visualTree.CloneTree(root);

            var listItem = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PackageUtils.VisualTreeFolder}/FigmaFileAsset_PageListItem.uxml");
            
            var listView = root.Q<ListView>("pagesList");
            listView.makeItem = () => listItem.Instantiate();
            listView.bindItem = (e, i) =>
            {
                var page = ((FigmaFileAsset) target).pages[i];
                var toggle = e.Q<Toggle>();
                toggle.label = page.name;
                toggle.value = page.enabled;
                toggle.RegisterValueChangedCallback(change =>
                {
                    page.enabled = change.newValue;
                    EditorUtility.SetDirty(target);
                });
            };

            root.Q<Button>("copyContentButton").clicked += () =>
            {
                var file = ((FigmaFileAsset) target);
                if (file.content != null)
                {
                    GUIUtility.systemCopyBuffer = file.content.text;   
                }
            };
            
            root.Q<Button>("exportFileButton").clicked += () => 
            {
                var file = (FigmaFileAsset) target;
                var path = EditorUtility.SaveFilePanel("Export Figma File", "", $"{file.id}.json", "json");
                
                if (!string.IsNullOrWhiteSpace(path))
                {
                    System.IO.File.WriteAllText(path, file.content.text);    
                }
            };
            
            return root;
        }
    }
}