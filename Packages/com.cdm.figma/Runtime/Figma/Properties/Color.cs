﻿using System.Runtime.Serialization;
using Newtonsoft.Json;

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
        [JsonProperty("a")]
        public float a { get; set; }

        /// <summary>
        /// Blue channel value, between 0 and 1.
        /// </summary>
        [JsonProperty("b")]
        public float b { get; set; }

        /// <summary>
        /// Green channel value, between 0 and 1.
        /// </summary>
        [JsonProperty("g")]
        public float g { get; set; }

        /// <summary>
        /// Red channel value, between 0 and 1.
        /// </summary>
        [JsonProperty("r")]
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

        public static implicit operator UnityEngine.Color(Color c) => new UnityEngine.Color(c.r, c.g, c.b, c.a);
        public static explicit operator Color(UnityEngine.Color c) => new Color(c.r, c.g, c.b, c.a);
    }
}