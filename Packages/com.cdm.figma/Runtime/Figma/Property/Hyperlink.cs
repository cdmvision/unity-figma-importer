using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// A link to either a URL or another frame (node) in the document.
    /// </summary>
    [Serializable]
    public class Hyperlink
    {
        /// <summary>
        /// Type of hyperlink.
        /// </summary>
        [JsonProperty("type")]
        public HyperlinkType type { get; set; }
        
        /// <summary>
        /// URL being linked to, if <see cref="HyperlinkType.Url"/> type.
        /// </summary>
        [JsonProperty("url")]
        public string url { get; set; }
        
        /// <summary>
        /// ID of frame hyperlink points to, if <see cref="HyperlinkType.Node"/> type
        /// </summary>
        [JsonProperty("nodeID")]
        public string nodeId { get; set; }
    }

    [Serializable]
    public enum HyperlinkType
    {
        [EnumMember(Value = "URL")]
        Url,
        
        [EnumMember(Value = "NODE")]
        Node
    }
}