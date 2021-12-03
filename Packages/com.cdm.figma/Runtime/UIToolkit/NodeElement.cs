using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class NodeElement
    {
        /// <summary>
        /// Actual XML element.
        /// </summary>
        public XElement value { get; }
        
        /// <summary>
        /// Style of the element.
        /// </summary>
        /// <remarks>
        /// Don't forget to call <see cref="UpdateStyle"/> whenever update the style.
        /// </remarks>
        public Style style { get; }
        
        private NodeElement(XName name, params object[] content)
        {
            value = new XElement(name, content);
            style = new Style();
            
            // Figma transform origin starts from top left corner.
            style.transformOrigin = new StyleTransformOrigin(
                new TransformOrigin(new Length(0, LengthUnit.Percent), new Length(0, LengthUnit.Percent), 0f));
        }

        /// <summary>
        /// Adds or sets the style attribute of the actual XML element.
        /// </summary>
        public void UpdateStyle()
        {
            value.SetAttributeValue("style", style.ToString());
        }

        /// <summary>
        /// Initializes a new instance of the XElement class with the specified <paramref name="node"/>.
        /// </summary>
        public static NodeElement New<T>(Node node, NodeConvertArgs args)
        {
            var nodeData = new NodeElement(args.namespaces.engine + typeof(T).Name, GetNodeId(node));
            
            var name = GetNodeName(node);
            nodeData.value.Add(name);
            
            var bindingId = GetBindingKey(node);
            if (bindingId != null)
            {
                nodeData.value.Add(bindingId);
            }
            
            var localizationId = GetLocalizationKey(node);
            if (localizationId != null)
            {
                nodeData.value.Add(localizationId);
            }
            
            return nodeData;
        }
        
        /// <summary>
        /// Gets the <see cref="Node.id"/> of the <paramref name="node"/> as XML attribute.
        /// </summary>
        private static XAttribute GetNodeId(Node node)
        {
            return new XAttribute("nodeId", node.id);
        }
        
        /// <summary>
        /// Gets the <see cref="Node.name"/> of the <paramref name="node"/> as XML attribute.
        /// </summary>
        private static XAttribute GetNodeName(Node node)
        {
            return new XAttribute("nodeName", node.name);
        }
        
        /// <summary>
        /// Gets binding identifier as XML attribute for the node if exist.
        /// </summary>
        private static XAttribute GetBindingKey(Node node)
        {
            var key = node.GetBindingKey();
            if (!string.IsNullOrEmpty(key))
            {
                return new XAttribute("name", key);
            }
            
            return null;
        }

        /// <summary>
        /// Gets localization identifier as XML attribute for the node if exist.
        /// </summary>
        private static XAttribute GetLocalizationKey(Node node)
        {
            var key = node.GetLocalizationKey();
            if (!string.IsNullOrEmpty(key))
            {
                return new XAttribute("lang", key);
            }

            return null;
        }
    }
}