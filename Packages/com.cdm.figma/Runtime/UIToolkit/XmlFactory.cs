using System.Linq;
using System.Xml.Linq;
using Cdm.Figma.UIToolkit;
using UnityEngine;

namespace Cdm.Figma
{
    public static class XmlFactory
    {
        /// <summary>
        /// Gets the <see cref="Node.id"/> of the <paramref name="node"/> as XML attribute.
        /// </summary>
        public static XAttribute GetNodeId(Node node)
        {
            return new XAttribute("nodeId", node.id);
        }
        
        /// <summary>
        /// Gets the <see cref="Node.name"/> of the <paramref name="node"/> as XML attribute.
        /// </summary>
        public static XAttribute GetNodeName(Node node)
        {
            return new XAttribute("nodeName", node.name);
        }
        
        /// <summary>
        /// Gets binding identifier as XML attribute for the node if exist.
        /// </summary>
        public static XAttribute GetBindingKey(Node node)
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
        public static XAttribute GetLocalizationKey(Node node)
        {
            var key = node.GetLocalizationKey();
            if (!string.IsNullOrEmpty(key))
            {
                return new XAttribute("lang", key);
            }

            return null;
        }

        /// <summary>
        /// Initializes a new instance of the XElement class with the specified name and content.
        /// </summary>
        public static XElement NewElement(NodeConvertArgs args, string name, params object[] content)
        {
            return new XElement(args.namespaces.engine + name, content);
        }
        
        /// <summary>
        /// Initializes a new instance of the XElement class with the specified type and content.
        /// </summary>
        public static XElement NewElement<T>(NodeConvertArgs args, params object[] content)
        {
            return new XElement(args.namespaces.engine + typeof(T).Name, content);
        }
        
        /// <summary>
        /// Initializes a new instance of the XElement class with the specified <paramref name="node"/>.
        /// </summary>
        public static XElement NewElement<T>(Node node, NodeConvertArgs args)
        {
            var element = NewElement<T>(args, GetNodeId(node));
            
            var name = GetNodeName(node);
            element.Add(name);
            
            var bindingId = GetBindingKey(node);
            if (bindingId != null)
            {
                element.Add(bindingId);
                
                // Remove binding identifier from node name?
                //name.Value = name.Value.Replace($"{args.importer.bindingPrefix}{bindingId.Value}", "");
            }
            
            var localizationId = GetLocalizationKey(node);
            if (localizationId != null)
            {
                element.Add(localizationId);
                
                // Remove localization identifier from node name?
                //name.Value = name.Value.Replace($"{args.importer.localizationPrefix}{localizationId.Value}", "");
            }
            
            return element;
        }

        public static XAttribute NewStyle(string style)
        {
            return new XAttribute("style", style);
        }

        public static XElement Style(this XElement element, string style)
        {
            element.Add(NewStyle(style));
            return element;
        }

        private static string GetSpecialId(Node node, string prefix)
        {
            var tokens = node.name.Split(" ");
            var token = tokens.FirstOrDefault(t => t.StartsWith(prefix));
            return token?.Replace(prefix, "");
        }
    }
}