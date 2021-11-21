using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// Information about the arc properties of an ellipse. 0° is the x axis and increasing angles rotate clockwise.
    /// </summary>
    [DataContract]
    public partial class ArcData
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