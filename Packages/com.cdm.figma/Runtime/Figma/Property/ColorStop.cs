using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// A position color pair representing a gradient stop.
    ///
    /// Positions of key points along the gradient axis with the colors
    /// anchored there. Colors along the gradient are interpolated smoothly
    /// between neighboring gradient stops.
    /// </summary>
    [Serializable]
    public partial class ColorStop
    {
        /// <summary>
        /// Value between 0 and 1 representing position along gradient axis.
        /// </summary>
        [JsonProperty("position")]
        public float position { get; set; }
        
        /// <summary>
        /// Color attached to corresponding position.
        /// </summary>
        [JsonProperty("color")]
        public Color color { get; set; }
    }
}