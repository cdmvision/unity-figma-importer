using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A description of a main component. Helps you identify which component instances are attached to.
    /// </summary>
    [DataContract]
    public class Component
    {
        /// <summary>
        /// The key of the component.
        /// </summary>
        [DataMember(Name = "key", IsRequired = true)]
        public string key { get; set; }

        /// <summary>
        /// The name of the component.
        /// </summary>
        [DataMember(Name = "name")]
        public string name { get; set; }

        /// <summary>
        /// The description of the component as entered in the editor
        /// </summary>
        [DataMember(Name = "description")]
        public string description { get; set; }
        
        [DataMember(Name = "componentSetId")]
        public string componentSetId { get; set; } = null;

        /// <summary>
        /// The documentation links for this component.
        /// </summary>
        [DataMember(Name = "documentationLinks")]
        public List<DocumentationLink> documentationLinks { get; private set; } = new List<DocumentationLink>();
    }
    
    /// <summary>
    /// Represents a link to documentation for a component.
    /// </summary>
    [DataContract]
    public class DocumentationLink
    {
        /// <summary>
        /// Should be a valid URI (e.g. https://www.figma.com).
        /// </summary>
        [DataMember(Name = "uri")]
        public string uri { get; set; }
    }
}