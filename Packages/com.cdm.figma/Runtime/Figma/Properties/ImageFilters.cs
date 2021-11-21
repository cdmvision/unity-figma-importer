using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// Defines the image filters applied to an image paint. All values are from -1 to 1.
    /// </summary>
    [Serializable]
    public class ImageFilters
    {
        [JsonProperty("exposure")]
        public float exposure { get; set; }

        [JsonProperty("contrast")]
        public float contrast { get; set; }

        [JsonProperty("saturation")]
        public float saturation { get; set; }

        [JsonProperty("temperature")]
        public float temperature { get; set; }

        [JsonProperty("tint")]
        public float tint { get; set; }

        [JsonProperty("highlights")]
        public float highlights { get; set; }

        [JsonProperty("shadows")]
        public float shadows { get; set; }
    }
}