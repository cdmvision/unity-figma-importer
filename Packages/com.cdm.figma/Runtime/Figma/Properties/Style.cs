using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A set of properties that can be applied to nodes and published. Styles for a property can be created in the
    /// corresponding property's panel while editing a file.
    /// </summary>
    [DataContract]
    public class Style
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
        /// The description of the component as entered in the editor.
        /// </summary>
        [DataMember(Name = "description")]
        public string description { get; set; }
        
        /// <summary>
        /// The type of style.
        /// </summary>
        [DataMember(Name = "style_type")]
        public StyleType styleType { get; set; }
    }

    [DataContract]
    public enum StyleType
    {
        [EnumMember(Value = "FILL")]
        Fill,
        
        [EnumMember(Value = "TEXT")]
        Text,
        
        [EnumMember(Value = "EFFECT")]
        Effect,
        
        [EnumMember(Value = "GRID")]
        Grid
    }
}