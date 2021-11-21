using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public partial class Project
    {
        [JsonProperty("id", Required = Required.Always)]
        public float id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }
    }
}