using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ShadowEffect : Effect
    {
        /// <summary>
        /// Radius of the effect.
        /// </summary>
        [DataMember(Name = "radius")]
        public float radius { get; set; }
        
        /// <summary>
        /// The color of the shadow.
        /// </summary>
        [DataMember(Name = "color")]
        public Color color { get; set; }
        
        /// <summary>
        /// Blend mode of the shadow.
        /// </summary>
        [DataMember(Name = "blendMode")]
        public BlendMode? blendMode { get; set; }
        
        /// <summary>
        /// How far the shadow is projected in the x and y directions.
        /// </summary>
        [DataMember(Name = "offset")]
        public Vector offset { get; set; }
        
        /// <summary>
        /// How far the shadow spreads.
        /// </summary>
        [DataMember(Name = "spread")]
        public float spread { get; set; } = 0f;
    }
}