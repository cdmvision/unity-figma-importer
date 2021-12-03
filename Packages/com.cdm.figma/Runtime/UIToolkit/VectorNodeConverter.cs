using System.Text;
using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class VectorNodeConverter : NodeConverter<VectorNode>
    {
        public static NodeData Convert(VectorNode node, NodeConvertArgs args)
        {
            var data = XmlFactory.NewElement<VisualElement>(node, args);
            BuildStyle(node, args, data);
            data.UpdateStyle();
            return data;
        }

        public override NodeData Convert(Node node, NodeConvertArgs args)
        {
            return Convert((VectorNode) node, args);
        }

        private static string BuildStyle(VectorNode node, NodeConvertArgs args, NodeData data)
        {
            var style = new StringBuilder();
            
            data.style.width = new StyleLength(node.size.x);
            data.style.height = new StyleLength(node.size.y);

            // Override width and height if SVG contains these properties.
            if (args.file.TryGetGraphic(node.id, out var graphic))
            {
                data.style.backgroundImage = new StyleBackground(graphic);
                
#if UNITY_EDITOR
                var path = UnityEditor.AssetDatabase.GetAssetPath(graphic);
                var svg = XDocument.Load(path);
                if (svg.Root != null)
                {
                    var widthAttribute = svg.Root.Attribute("width");
                    if (widthAttribute != null)
                    {
                        data.style.width = new StyleLength(float.Parse(widthAttribute.Value));
                    }

                    var heightAttribute = svg.Root.Attribute("height");
                    if (heightAttribute != null)
                    {
                        data.style.height = new StyleLength(float.Parse(heightAttribute.Value));
                    }
                }
#else
                Debug.LogWarning("SVG file cannot be read at runtime.");
#endif
            }

            return style.ToString();
        }
    }
}