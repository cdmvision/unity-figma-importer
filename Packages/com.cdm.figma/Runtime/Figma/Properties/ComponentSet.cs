using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class ComponentSet
    {
        /// <summary>
        /// The key of the component.
        /// </summary>
        [JsonProperty("key", Required = Required.Always)]
        public string key { get; set; }

        /// <summary>
        /// The name of the component.
        /// </summary>
        [JsonProperty("name")]
        public string name { get; set; }

        /// <summary>
        /// The description of the component as entered in the editor
        /// </summary>
        [JsonProperty("description")]
        public string description { get; set; }
    }
}