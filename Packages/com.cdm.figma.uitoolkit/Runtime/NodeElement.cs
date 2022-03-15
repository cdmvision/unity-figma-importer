using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class NodeElement
    {
        private static class Attributes
        {
            public const string NodeId = "nodeId";
            public const string NodeName = "nodeName";
            public const string ElementName = "name";
            public const string LocalizationKey = "binding-path";
        }
        
        /// <summary>
        /// The actual node.
        /// </summary>
        public Node node { get; }
        
        /// <summary>
        /// Actual XML element.
        /// </summary>
        public XElement value { get; }

        /// <summary>
        /// Inline style of the element.
        /// </summary>
        public Style inlineStyle { get; private set; }

        /// <summary>
        /// Style definitions that will be written on a separate style file. 
        /// </summary>
        public List<StyleDefinition> styles { get; } = new List<StyleDefinition>();

        /// <summary>
        /// Gets the parent node element of this node.
        /// </summary>
        public NodeElement parent { get; private set; }
        
        public Type elementType { get; private set; }
        public string elementName { get; private set; }
        public string localizationKey { get; private set; }

        private readonly List<NodeElement> _children = new List<NodeElement>();
        
        /// <summary>
        /// Gets the children of this node element.
        /// </summary>
        public IReadOnlyList<NodeElement> children => _children;
        
        public bool isRootElement => parent == null;
        public bool isLeafElement => _children.Count == 0;
        
        private NodeElement(Node node, XName name, params object[] content)
        {
            this.node = node;
            value = new XElement(name, content);

            ClearStyle();
        }

        public void AddChild(NodeElement child)
        {
            child.parent = this;
            _children.Add(child);
        }

        public void ClearStyle()
        {
            inlineStyle = new Style();

            // Figma transform origin starts from top left corner.
            inlineStyle.transformOrigin = new StyleTransformOrigin(
                new TransformOrigin(new Length(0, LengthUnit.Percent), new Length(0, LengthUnit.Percent), 0f));
        }

        /// <summary>
        /// Initializes a new instance of the XElement class with the specified <paramref name="node"/>.
        /// </summary>
        public static NodeElement New<T>(Node node, NodeConvertArgs args) where T : VisualElement
        {
            var nodeElement = new NodeElement(node, args.namespaces.engine + typeof(T).Name);

            nodeElement.elementType = typeof(T);
            nodeElement.elementName = node.GetBindingName();
            nodeElement.localizationKey = node.GetLocalizationKey();
            
            nodeElement.value.Add(new XAttribute(Attributes.NodeId, node.id));
            nodeElement.value.Add( new XAttribute(Attributes.NodeName, node.name));
            
            if (!string.IsNullOrEmpty(nodeElement.elementName))
            {
                nodeElement.value.Add(new XAttribute(Attributes.ElementName, nodeElement.elementName));
            }
            
            if (!string.IsNullOrEmpty(nodeElement.localizationKey))
            {
                nodeElement.value.Add(new XAttribute(Attributes.LocalizationKey, nodeElement.localizationKey));
            }

            return nodeElement;
        }
    }
}