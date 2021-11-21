using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class EllipseNode : VectorNode
    {
        public override NodeType type => NodeType.Ellipse;
        
        /// <summary>
        /// Start and end angles of the ellipse measured clockwise from the x axis, plus the inner radius for donuts.
        /// </summary>
        [JsonProperty("arcData")]
        public ArcData arcData { get; set; }
    }
    
    /// <summary>
    /// Information about the arc properties of an ellipse. 0° is the x axis and increasing angles rotate clockwise.
    /// </summary>
    [Serializable]
    public class ArcData
    {
        /// <summary>
        /// Start of the sweep in radians.
        /// </summary>
        [JsonProperty("startingAngle")]
        public float startingAngle { get; set; }
        
        /// <summary>
        /// End of the sweep in radians.
        /// </summary>
        [JsonProperty("endingAngle")]
        public float endingAngle { get; set; }
        
        /// <summary>
        /// Inner radius value between 0 and 1.
        /// </summary>
        [JsonProperty("innerRadius")]
        public float innerRadius { get; set; }
    }
}