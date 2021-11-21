using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// Node Properties
    /// The root node
    ///
    /// The root node within the document
    /// </summary>
    [DataContract]
    public class RootNode
    {
        /// <summary>
        /// Unique identifier of the node within the document.
        /// </summary>
        [JsonProperty("id", Required = Required.Always)]
        public string id { get; set; }

        /// <summary>
        /// The type of the node, refer to table below for details.
        /// </summary>
        [JsonProperty("type", Required = Required.Always)]
        public NodeType type { get; set; }

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
        /// A list of canvases attached to the document.
        /// </summary>
        [JsonProperty("children")]
        public List<Node> children { get; private set; } = new List<Node>();
    }
}