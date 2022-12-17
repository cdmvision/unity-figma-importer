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
        public virtual string type { get; private set; }

        /// <summary>
        /// Is the effect active?
        /// </summary>
        [DataMember(Name = "visible")]
        public bool visible { get; set; } = true;
    }
    
    public class EffectType
    {
        public const string InnerShadow = "INNER_SHADOW";
        public const string DropShadow = "DROP_SHADOW";
        public const string LayerBlur = "LAYER_BLUR";
        public const string BackgroundBlur = "BACKGROUND_BLUR";
    }
}