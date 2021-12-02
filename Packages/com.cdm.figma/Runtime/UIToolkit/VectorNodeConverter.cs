using System.Globalization;
using System.Text;
using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class VectorNodeConverter : NodeConverter<VectorNode>
    {
        public static XElement Convert(VectorNode node, NodeConvertArgs args)
        {
            var style = BuildStyle(node, args);
            return XmlFactory.NewElement<VisualElement>(node, args).Style(style);
        }

        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return Convert((VectorNode) node, args);
        }

        private static string BuildStyle(VectorNode node, NodeConvertArgs args)
        {
            var style = new StringBuilder();

            // Size and position.
            var absoluteBoundingBox = node.absoluteBoundingBox;
            var width = absoluteBoundingBox.width.ToString(CultureInfo.InvariantCulture);
            var height = absoluteBoundingBox.height.ToString(CultureInfo.InvariantCulture);

            // Override width and height if size property is exist.
            if (node.size != null)
            {
                width = node.size.x.ToString(CultureInfo.InvariantCulture);
                height = node.size.y.ToString(CultureInfo.InvariantCulture);
            }

            // Override width and height if SVG contains these properties.
            if (args.file.TryGetGraphic(node.id, out var graphic))
            {
#if UNITY_EDITOR
                var path = UnityEditor.AssetDatabase.GetAssetPath(graphic);
                var svg = XDocument.Load(path);
                if (svg.Root != null)
                {
                    var widthAttribute = svg.Root.Attribute("width");
                    if (widthAttribute != null)
                    {
                        width = widthAttribute.Value;
                    }

                    var heightAttribute = svg.Root.Attribute("height");
                    if (heightAttribute != null)
                    {
                        height = heightAttribute.Value;
                    }
                }
#else
                Debug.LogWarning("SVG file cannot be read at runtime.");
#endif
            }

            style.Append($"width: {width}px; ");
            style.Append($"height: {height}px; ");

            if (args.file.TryGetGraphicUrl(node.id, out var url))
            {
                style.Append($"background-image: {url}; ");
            }

            return style.ToString();
        }
    }
}