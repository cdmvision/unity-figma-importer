using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// A set of properties that can be applied to nodes and published. Styles for a property can be created in the
    /// corresponding property's panel while editing a file.
    /// </summary>
    [Serializable]
    public class Style
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
        /// The description of the component as entered in the editor.
        /// </summary>
        [JsonProperty("description")]
        public string description { get; set; }
        
        /// <summary>
        /// The type of style.
        /// </summary>
        [JsonProperty("style_type")]
        public StyleType styleType { get; set; }
    }

    [Serializable]
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