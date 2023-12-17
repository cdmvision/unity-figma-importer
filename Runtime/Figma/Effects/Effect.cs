using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A visual effect such as a shadow or blur.
    /// </summary>
    [DataContract]
    public class Effect
    {
        /// <summary>
        /// Type of effect.
        /// </summary>
        /// <seealso cref="EffectType"/>
        [DataMember(Name = "type", IsRequired = true)]
        public virtual EffectType type { get; private set; }

        /// <summary>
        /// Is the effect active?
        /// </summary>
        [DataMember(Name = "visible")]
        public bool visible { get; set; } = true;
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