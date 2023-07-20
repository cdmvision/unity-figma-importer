using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentSet
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
    }
}