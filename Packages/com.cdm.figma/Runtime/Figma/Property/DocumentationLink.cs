using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// Represents a link to documentation for a component.
    /// </summary>
    [Serializable]
    public class DocumentationLink
    {
        /// <summary>
        /// Should be a valid URI (e.g. https://www.figma.com).
        /// </summary>
        [JsonProperty("uri")]
        public string uri { get; set; }
    }
}