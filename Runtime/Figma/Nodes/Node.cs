using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cdm.Figma.Json;
using Newtonsoft.Json;
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
        public virtual NodeType type { get; set; }

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
    [JsonConverter(typeof(DefaultUnknownEnumConverter), Unknown)]
    [DataContract]
    public enum NodeType
    {
        Unknown,
        
        [EnumMember(Value = "BOOLEAN_OPERATION")]
        Boolean,

        [EnumMember(Value = "CANVAS")]
        Page,

        [EnumMember(Value = "COMPONENT")]
        Component,

        [EnumMember(Value = "COMPONENT_SET")]
        ComponentSet,

        [EnumMember(Value = "DOCUMENT")]
        Document,

        [EnumMember(Value = "ELLIPSE")]
        Ellipse,

        [EnumMember(Value = "FRAME")]
        Frame,

        [EnumMember(Value = "GROUP")]
        Group,

        [EnumMember(Value = "INSTANCE")]
        Instance,

        [EnumMember(Value = "LINE")]
        Line,

        [EnumMember(Value = "RECTANGLE")]
        Rectangle,

        [EnumMember(Value = "REGULAR_POLYGON")]
        Polygon,

        [EnumMember(Value = "SLICE")]
        Slice,

        [EnumMember(Value = "STAR")]
        Star,

        [EnumMember(Value = "TEXT")]
        Text,

        [EnumMember(Value = "VECTOR")]
        Vector,
        
        [EnumMember(Value = "SECTION")]
        Section
    }
}