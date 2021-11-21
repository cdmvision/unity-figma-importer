using System;
using System.Runtime.Serialization;
using JsonSubTypes;
using Newtonsoft.Json.Linq;
using Unity.Plastic.Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(BooleanOperationNode), NodeType.Boolean)]
    [JsonSubtypes.KnownSubType(typeof(CanvasNode), NodeType.Canvas)]
    [JsonSubtypes.KnownSubType(typeof(ComponentNode), NodeType.Component)]
    [JsonSubtypes.KnownSubType(typeof(ComponentSetNode), NodeType.ComponentSet)]
    [JsonSubtypes.KnownSubType(typeof(DocumentNode), NodeType.Document)]
    [JsonSubtypes.KnownSubType(typeof(EllipseNode), NodeType.Ellipse)]
    [JsonSubtypes.KnownSubType(typeof(FrameNode), NodeType.Frame)]
    [JsonSubtypes.KnownSubType(typeof(GroupNode), NodeType.Group)]
    [JsonSubtypes.KnownSubType(typeof(InstanceNode), NodeType.Instance)]
    [JsonSubtypes.KnownSubType(typeof(RectangleNode), NodeType.Rectangle)]
    [JsonSubtypes.KnownSubType(typeof(RegularPolygonNode), NodeType.RegularPolygon)]
    [JsonSubtypes.KnownSubType(typeof(SliceNode), NodeType.Slice)]
    [JsonSubtypes.KnownSubType(typeof(StarNode), NodeType.Star)]
    [JsonSubtypes.KnownSubType(typeof(TextNode), NodeType.Text)]
    [JsonSubtypes.KnownSubType(typeof(VectorNode), NodeType.Vector)]
    public class Node
    {
        /// <summary>
        /// A string uniquely identifying this node within the document.
        /// </summary>
        [JsonProperty("id", Required = Required.Always)]
        public string id { get; set; }

        /// <summary>
        /// The name given to the node by the user in the tool.
        /// </summary>
        [JsonProperty("name")]
        public string name { get; set; }

        /// <summary>
        /// Whether or not the node is visible on the canvas.
        /// </summary>
        [JsonProperty("visible")]
        public bool visible { get; set; } = true;

        /// <summary>
        /// The type of the node, refer to table below for details.
        /// </summary>
        [JsonProperty("type", Required = Required.Always)]
        public virtual NodeType type { get; private set; }

        /// <summary>
        /// Data written by plugins that is visible only to the plugin that wrote it. Requires the `pluginData` to
        /// include the ID of the plugin.
        /// </summary>
        [JsonProperty("pluginData")]
        public JToken pluginData { get; set; }

        /// <summary>
        /// Data written by plugins that is visible to all plugins. Requires the `pluginData` parameter to include
        /// the string "shared".
        /// </summary>
        [JsonProperty("sharedPluginData")]
        public JToken sharedPluginData { get; set; }

        public virtual Node[] GetChildren() => null;
    }

    /// <summary>
    /// The type of the node, refer to table below for details.
    /// </summary>
    [Serializable]
    public enum NodeType
    {
        [EnumMember(Value = "BOOLEAN_OPERATION")]
        Boolean,

        [EnumMember(Value = "CANVAS")]
        Canvas,

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
        RegularPolygon,

        [EnumMember(Value = "SLICE")]
        Slice,

        [EnumMember(Value = "STAR")]
        Star,

        [EnumMember(Value = "TEXT")]
        Text,

        [EnumMember(Value = "VECTOR")]
        Vector
    }
}