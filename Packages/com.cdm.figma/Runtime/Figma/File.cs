using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public partial class File
    {
        [JsonProperty("key")]
        public string key { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("thumbnail_url")]
        public string thumbnailUrl { get; set; }

        /// <summary>
        /// UTC date in iso8601.
        /// </summary>
        [JsonProperty("last_modified")]
        public string lastModified { get; set; }
    }
}