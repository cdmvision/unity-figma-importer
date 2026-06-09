using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class BlurEffect : Effect
    {
        /// <summary>
        /// Radius of the blur effect.
        /// </summary>
        [DataMember(Name = "radius")]
        public float radius { get; set; }
    }
}