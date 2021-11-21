using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// A 2d vector
    ///
    /// This field contains three vectors, each of which are a position in
    /// normalized object space (normalized object space is if the top left
    /// corner of the bounding box of the object is (0, 0) and the bottom
    /// right is (1,1)). The first position corresponds to the start of the
    /// gradient (value 0 for the purposes of calculating gradient stops),
    /// the second position is the end of the gradient (value 1), and the
    /// third handle position determines the width of the gradient (only
    /// relevant for non-linear gradients).
    ///
    /// 2d vector offset within the frame.
    ///
    /// A relative offset within a frame
    /// </summary>
    [Serializable]
    public partial class CommentMetadata
    {
        /// <summary>
        /// X coordinate of the vector.
        /// </summary>
        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public float? x { get; set; }

        /// <summary>
        /// Y coordinate of the vector.
        /// </summary>
        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public float? y { get; set; }

        /// <summary>
        /// Unique id specifying the frame.
        /// </summary>
        [JsonProperty("node_id", NullValueHandling = NullValueHandling.Ignore)]
        public string[] nodeId { get; set; }

        /// <summary>
        /// 2d vector offset within the frame.
        /// </summary>
        [JsonProperty("node_offset", NullValueHandling = NullValueHandling.Ignore)]
        public Vector nodeOffset { get; set; }
    }
}