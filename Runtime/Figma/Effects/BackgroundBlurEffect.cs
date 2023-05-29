using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class BackgroundBlurEffect : BlurEffect
    {
        public override string type => EffectType.BackgroundBlur;
    }
}