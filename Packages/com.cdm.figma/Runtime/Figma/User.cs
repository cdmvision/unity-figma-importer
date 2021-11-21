using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// The user who left the comment
    ///
    /// A description of a user
    /// </summary>
    [Serializable]
    public partial class User
    {
        [JsonProperty("handle")]
        public string handle { get; set; }

        [JsonProperty("img_url")]
        public string imageUrl { get; set; }
    }
}