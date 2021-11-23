using UnityEditor;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CustomEditor(typeof(ComponentConverter), editorForChildClasses: true)]
    public class ComponentConverterEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                $"{PackageUtils.VisualTreeFolder}/UIToolkit/ComponentConverter.uxml");
            visualTree.CloneTree(root);
            
            var stateListViewItem = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{PackageUtils.VisualTreeFolder}/UIToolkit/ComponentConverter_StateListItem.uxml");
            
            var stateListView = root.Q<ListView>("stateList");
            stateListView.makeItem = () => stateListViewItem.Instantiate();
            stateListView.bindItem = (e, i) =>
            {
                var componentState = ((ComponentConverter) target).states[i];

                var textField = e.Q<TextField>();
                textField.label = componentState.state;
                textField.value = componentState.value;

                textField.RegisterValueChangedCallback(change =>
                {
                    componentState.value = change.newValue;
                    EditorUtility.SetDirty(target);
                });
            };

            return root;
        }
    }
}