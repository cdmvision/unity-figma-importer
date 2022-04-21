using UnityEditor;
using UnityEditor.UIElements;
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
                $"{EditorHelper.VisualTreeFolderPath}/ComponentConverter.uxml");
            visualTree.CloneTree(root);

            var propertyListViewItem =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorHelper.VisualTreeFolderPath}/ComponentConverter_VariantListItem.uxml");

            var propertyListView = root.Q<ListView>("variantList");
            propertyListView.makeItem = () => propertyListViewItem.Instantiate();
            propertyListView.bindItem = (e, i) =>
            {
                var property = serializedObject.FindProperty("_properties").GetArrayElementAtIndex(i);
                var propertyKey = property.FindPropertyRelative("key");
                var propertyVariants = property.FindPropertyRelative("variants");

                var textField = e.Q<TextField>();
                textField.label = propertyKey.displayName;
                textField.BindProperty(propertyKey);

                var propertyVariantListViewItem =
                    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                        $"{EditorHelper.VisualTreeFolderPath}/ComponentConverter_VariantPropertyListItem.uxml");

                var propertyVariantListView = e.Q<ListView>();
                propertyVariantListView.makeItem = () => propertyVariantListViewItem.Instantiate();
                propertyVariantListView.bindItem = (se, si) =>
                {
                    var variantProperty = propertyVariants.GetArrayElementAtIndex(si);

                    var tf = se.Q<TextField>();
                    tf.label = variantProperty.FindPropertyRelative("key").stringValue;
                    tf.BindProperty(variantProperty.FindPropertyRelative("value"));
                };
                propertyVariantListView.BindProperty(propertyVariants);
            };

            return root;
        }
    }
}