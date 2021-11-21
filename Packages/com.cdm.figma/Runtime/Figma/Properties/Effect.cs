using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// A visual effect such as a shadow or blur.
    /// 
    /// An array of effects attached to this node
    /// (see effects section for more details)
    /// </summary>
    [Serializable]
    public partial class Effect
    {
        /// <summary>
        /// Type of effect.
        /// </summary>
        [JsonProperty("type")]
        public EffectType type { get; set; }
        
        /// <summary>
        /// Is the effect active?
        /// </summary>
        [JsonProperty("visible", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool visible { get; set; } = true;
        
        /// <summary>
        /// Radius of the blur effect (applies to shadows as well)
        /// </summary>
        [JsonProperty("radius")]
        public float radius { get; set; }
        
        /// <summary>
        /// The color of the shadow.
        /// </summary>
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public Color color { get; set; }
        
        /// <summary>
        /// Blend mode of the shadow.
        /// </summary>
        [JsonProperty("blendMode", NullValueHandling = NullValueHandling.Ignore)]
        public BlendMode? blendMode { get; set; }

        /// <summary>
        /// How far the shadow is projected in the x and y directions.
        /// </summary>
        [JsonProperty("offset", NullValueHandling = NullValueHandling.Ignore)]
        public Vector offset { get; set; }

        /// <summary>
        /// How far the shadow spreads.
        /// </summary>
        [JsonProperty("spread")]
        public float spread { get; set; } = 0f;

        /// <summary>
        /// Whether to show the shadow behind translucent or transparent pixels (applies only to drop shadows).
        /// </summary>
        [JsonProperty("showShadowBehindNode", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool showShadowBehindNode { get; set; } = false;
    }
    
    [Serializable]
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