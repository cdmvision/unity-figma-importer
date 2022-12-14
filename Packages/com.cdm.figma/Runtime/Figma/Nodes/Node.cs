using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Cdm.Figma
{
    /// <summary>
    /// In Figma, the Node is the basis for representing layers. There are many different types of nodes,
    /// each with their own set of properties.
    /// </summary>
    [DataContract]
    public class Node
    {
        /// <summary>
        /// A string uniquely identifying this node within the document.
        /// </summary>
        [DataMember(Name = "id", IsRequired = true)]
        public string id { get; set; }

        /// <summary>
        /// The name given to the node by the user in the tool.
        /// </summary>
        [DataMember(Name = "name")]
        public string name { get; set; }

        /// <summary>
        /// The type of the node, refer to table below for details.
        /// </summary>
        /// <seealso cref="NodeType"/>
        [DataMember(Name = "type", IsRequired = true)]
        public virtual string type { get; private set; }

        /// <summary>
        /// Data written by plugins that is visible only to the plugin that wrote it. Requires the `pluginData` to
        /// include the ID of the plugin.
        /// </summary>
        [DataMember(Name = "pluginData")]
        public Dictionary<string, JObject> pluginData { get; set; }
        
        /// <summary>
        /// Data written by plugins that is visible to all plugins. Requires the `pluginData` parameter to include
        /// the string "shared".
        /// </summary>
        [DataMember(Name = "sharedPluginData")]
        public Dictionary<string, JObject> sharedPluginData { get; set; }
        
        /// <summary>
        /// A mapping of a layer's property to component property name of component properties attached to this node.
        /// The component property name can be used to look up more information on the node's containing
        /// component node's <see cref="ComponentNode.componentPropertyDefinitions"/> or
        /// component set node's <see cref="ComponentSetNode.componentPropertyDefinitions"/>.
        /// </summary>
        [DataMember(Name = "componentPropertyReferences")]
        public ComponentPropertyReferences componentPropertyReferences { get; set; }

        /// <summary>
        /// Parent of the node.
        /// </summary>
        public Node parent { get; set; }

        /// <summary>
        /// Gets whether node has parent or not.
        /// </summary>
        public bool hasParent => parent != null;

        /// <summary>
        /// Gets whether node has children or not.
        /// </summary>
        public bool hasChildren => GetChildren() != null;

        /// <summary>
        /// Gets the children of the node if exist.
        /// </summary>
        /// <returns>Array of nodes if exist; otherwise, <c>null</c>.</returns>
        public virtual Node[] GetChildren() => Array.Empty<Node>();

        public override string ToString()
        {
            return $"[{id}, '{name}']";
        }
    }

    /// <summary>
    /// The type of the node, refer to table below for details.
    /// </summary>
    public class NodeType
    {
        public const string Boolean = "BOOLEAN_OPERATION";
        public const string Page = "CANVAS";
        public const string Component = "COMPONENT";
        public const string ComponentSet = "COMPONENT_SET";
        public const string Document = "DOCUMENT";
        public const string Ellipse = "ELLIPSE";
        public const string Frame = "FRAME";
        public const string Group = "GROUP";
        public const string Instance = "INSTANCE";
        public const string Line = "LINE";
        public const string Rectangle = "RECTANGLE";
        public const string Polygon = "REGULAR_POLYGON";
        public const string Slice = "SLICE";
        public const string Star = "STAR";
        public const string Text = "TEXT";
        public const string Vector = "VECTOR";    
    }
}