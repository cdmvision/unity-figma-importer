using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class VectorNodeConverter : NodeConverter<VectorNode>
    {
        public static NodeElement Convert(VectorNode node, NodeConvertArgs args)
        {
            var element = NodeElement.New<VisualElement>(node, args);
            BuildStyle(node, args, element.style);
            element.UpdateStyle();
            return element;
        }

        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            return Convert((VectorNode) node, args);
        }

        private static void BuildStyle(VectorNode node, NodeConvertArgs args, Style style)
        {
            style.width = new StyleLength(node.size.x);
            style.height = new StyleLength(node.size.y);

            // Override width and height if SVG contains these properties.
            if (args.file.TryGetGraphic(node.id, out var graphic))
            {
                style.backgroundImage = new StyleBackground(graphic);
                
#if UNITY_EDITOR
                var path = UnityEditor.AssetDatabase.GetAssetPath(graphic);
                var svg = XDocument.Load(path);
                if (svg.Root != null)
                {
                    var widthAttribute = svg.Root.Attribute("width");
                    if (widthAttribute != null)
                    {
                        style.width = new StyleLength(float.Parse(widthAttribute.Value));
                    }

                    var heightAttribute = svg.Root.Attribute("height");
                    if (heightAttribute != null)
                    {
                        style.height = new StyleLength(float.Parse(heightAttribute.Value));
                    }
                }
#else
                Debug.LogWarning("SVG file cannot be read at runtime.");
#endif
            }
        }
    }
}