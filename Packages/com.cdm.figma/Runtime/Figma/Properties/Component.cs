using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// A description of a main component. Helps you identify which component instances are attached to.
    /// </summary>
    [Serializable]
    public partial class Component
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
        
        [JsonProperty("componentSetId")]
        public string componentSetId { get; set; } = null;

        /// <summary>
        /// The documentation links for this component.
        /// </summary>
        [JsonProperty("documentationLinks")]
        public List<DocumentationLink> documentationLinks { get; private set; } = new List<DocumentationLink>();
    }
    
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