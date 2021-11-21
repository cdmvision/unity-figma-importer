using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// A relative offset within a frame.
    /// </summary>
    [Serializable]
    public class FrameOffset
    {
        /// <summary>
        /// Unique id specifying the frame.
        /// </summary>
        [JsonProperty("node_id")]
        public string nodeId { get; set; }
        
        /// <summary>
        /// 2D vector offset within the frame.
        /// </summary>
        [JsonProperty("node_offset")]
        public Vector nodeOffset { get; set; }
    }
}