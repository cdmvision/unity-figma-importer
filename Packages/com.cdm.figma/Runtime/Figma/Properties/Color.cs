using System.Runtime.Serialization;
using UnityEngine;

namespace Cdm.Figma
{
    /// <summary>
    /// Color component with 4-channels.
    /// </summary>
    [DataContract]
    public class Color
    {
        /// <summary>
        /// Alpha channel value, between 0 and 1.
        /// </summary>
        [DataMember(Name = "a")]
        public float a { get; set; }

        /// <summary>
        /// Blue channel value, between 0 and 1.
        /// </summary>
        [DataMember(Name = "b")]
        public float b { get; set; }

        /// <summary>
        /// Green channel value, between 0 and 1.
        /// </summary>
        [DataMember(Name = "g")]
        public float g { get; set; }

        /// <summary>
        /// Red channel value, between 0 and 1.
        /// </summary>
        [DataMember(Name = "r")]
        public float r { get; set; }

        public Color()
        {
        }

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public override string ToString()
        {
            return $"#{ColorUtility.ToHtmlStringRGBA((UnityEngine.Color) this)}";
        }

        public static implicit operator UnityEngine.Color(Color c) => new UnityEngine.Color(c.r, c.g, c.b, c.a);
        public static implicit operator Color(UnityEngine.Color c) => new Color(c.r, c.g, c.b, c.a);
    }
}