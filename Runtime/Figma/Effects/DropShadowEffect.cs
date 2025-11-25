using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class DropShadowEffect : ShadowEffect
    {
        public override EffectType type => EffectType.DropShadow;
        
        /// <summary>
        /// Whether to show the shadow behind translucent or transparent pixels.
        /// </summary>
        [DataMember(Name = "showShadowBehindNode")]
        public bool showShadowBehindNode { get; set; } = false;
    }
}