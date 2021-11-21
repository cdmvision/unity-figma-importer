using System.Runtime.Serialization;
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
    }
}