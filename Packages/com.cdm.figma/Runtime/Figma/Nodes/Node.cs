using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Cdm.Figma
{
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
        /// Whether or not the node is visible on the canvas.
        /// </summary>
        [DataMember(Name = "visible")]
        public bool visible { get; set; } = true;

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
        public JToken pluginData { get; set; }

        /// <summary>
        /// Data written by plugins that is visible to all plugins. Requires the `pluginData` parameter to include
        /// the string "shared".
        /// </summary>
        [DataMember(Name = "sharedPluginData")]
        public JToken sharedPluginData { get; set; }

        public virtual Node[] GetChildren() => null;
    }

    /// <summary>
    /// The type of the node, refer to table below for details.
    /// </summary>
    public class NodeType
    {
        public const string Boolean = "BOOLEAN_OPERATION";
        public const string Canvas = "CANVAS";
        public const string Component = "COMPONENT";
        public const string ComponentSet = "COMPONENT_SET";
        public const string Document = "DOCUMENT";
        public const string Ellipse = "ELLIPSE";
        public const string Frame = "FRAME";
        public const string Group = "GROUP";
        public const string Instance = "INSTANCE";
        public const string Line = "LINE";
        public const string Rectangle = "RECTANGLE";
        public const string RegularPolygon = "REGULAR_POLYGON";
        public const string Slice = "SLICE";
        public const string Star = "STAR";
        public const string Text = "TEXT";
        public const string Vector = "VECTOR";    
    }
}