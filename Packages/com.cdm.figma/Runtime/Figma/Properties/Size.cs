using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class Size
    {
        /// <summary>
        /// The width of a size.
        /// </summary>
        [JsonProperty("width")]
        public float width { get; set; }

        /// <summary>
        /// The height of a size.
        /// </summary>
        [JsonProperty("height")]
        public float height { get; set; }
    }
}