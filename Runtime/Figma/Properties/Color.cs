using System;
using System.Globalization;
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
            return ToString("rgba-hex");
        }

        public string ToString(string format)
        {
            var c = ((Color32) (UnityEngine.Color) this);
            
            switch (format)
            {
                case "rgb":
                    return $"rgb({c.r}, {c.g}, {c.b})";
                case "rgba":
                    return $"rgba({c.r}, {c.g}, {c.b}, {a.ToString(CultureInfo.InvariantCulture)})";
                case "rgb-hex":
                    return $"#{ColorUtility.ToHtmlStringRGB((UnityEngine.Color) this)}";
                case "rgba-hex":
                    return $"#{ColorUtility.ToHtmlStringRGBA((UnityEngine.Color) this)}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }
        }
        
        public static implicit operator UnityEngine.Color(Color c) => new UnityEngine.Color(c.r, c.g, c.b, c.a);
        public static implicit operator Color(UnityEngine.Color c) => new Color(c.r, c.g, c.b, c.a);
    }
}