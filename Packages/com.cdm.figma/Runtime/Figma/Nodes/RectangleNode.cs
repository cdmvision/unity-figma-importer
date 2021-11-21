using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class RectangleNode : VectorNode
    {
        public override NodeType type => NodeType.Rectangle;
        
        /// <summary>
        /// Radius of each corner of the frame if a single radius is set for all corners.
        /// </summary>
        [JsonProperty("cornerRadius")]
        public float? cornerRadius { get; set; }
        
        /// <summary>
        /// Array of length 4 of the radius of each corner of the frame, starting in the top left and
        /// proceeding clockwise.
        /// </summary>
        [JsonProperty("rectangleCornerRadii")]
        public float[] rectangleCornerRadii { get; set; }
    }
}