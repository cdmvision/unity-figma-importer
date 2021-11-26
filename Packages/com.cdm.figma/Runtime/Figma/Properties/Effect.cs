using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A visual effect such as a shadow or blur.
    /// 
    /// An array of effects attached to this node
    /// (see effects section for more details)
    /// </summary>
    [DataContract]
    public class Effect
    {
        /// <summary>
        /// Type of effect.
        /// </summary>
        [DataMember(Name = "type")]
        public EffectType type { get; set; }
        
        /// <summary>
        /// Is the effect active?
        /// </summary>
        [DataMember(Name = "visible")]
        public bool visible { get; set; } = true;
        
        /// <summary>
        /// Radius of the blur effect (applies to shadows as well)
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

        /// <summary>
        /// Whether to show the shadow behind translucent or transparent pixels (applies only to drop shadows).
        /// </summary>
        [DataMember(Name = "showShadowBehindNode")]
        public bool showShadowBehindNode { get; set; } = false;
    }
    
    [DataContract]
    public enum EffectType
    {
        [EnumMember(Value = "INNER_SHADOW")]
        InnerShadow,
        
        [EnumMember(Value = "DROP_SHADOW")]
        DropShadow,

        [EnumMember(Value = "LAYER_BLUR")]
        LayerBlur,
        
        [EnumMember(Value = "BACKGROUND_BLUR")]
        BackgroundBlur,
    }
}