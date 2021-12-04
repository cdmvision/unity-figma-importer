using System.Collections.Generic;
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
        /// Inline style of the element.
        /// </summary>
        public Style inlineStyle { get; }

        /// <summary>
        /// Style definitions that will be written on a separate style file. 
        /// </summary>
        public List<StyleDefinition> styles { get; } = new List<StyleDefinition>();

        /// <summary>
        /// Gets the parent node element of this node.
        /// </summary>
        public NodeElement parent { get; private set; }
        
        private readonly List<NodeElement> _children = new List<NodeElement>();
        
        /// <summary>
        /// Gets the children of this node element.
        /// </summary>
        public IReadOnlyList<NodeElement> children => _children;
        
        public bool isRootElement => parent == null;
        public bool isLeafElement => _children.Count == 0;
        
        private NodeElement(XName name, params object[] content)
        {
            value = new XElement(name, content);
            inlineStyle = new Style();

            // Figma transform origin starts from top left corner.
            inlineStyle.transformOrigin = new StyleTransformOrigin(
                new TransformOrigin(new Length(0, LengthUnit.Percent), new Length(0, LengthUnit.Percent), 0f));
        }

        public void AddChild(NodeElement child)
        {
            child.parent = this;
            _children.Add(child);
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