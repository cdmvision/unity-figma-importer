using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// Sizing constraint for exports.
    /// </summary>
    [Serializable]
    public partial class Constraint
    {
        /// <summary>
        /// Type of constraint to apply.
        /// </summary>
        [JsonProperty("type")]
        public ConstraintType type { get; set; }

        /// <summary>
        /// See type property for effect of this field.
        /// </summary>
        [JsonProperty("value")]
        public float value { get; set; }
    }
    
    /// <summary>
    /// Type of constraint to apply.
    /// </summary>
    [Serializable]
    public enum ConstraintType
    {
        /// <summary>
        /// Scale proportionally and set height to value.
        /// </summary>
        [EnumMember(Value = "HEIGHT")]
        Height,

        /// <summary>
        /// Scale by value.
        /// </summary>
        [EnumMember(Value = "SCALE")]
        Scale,

        /// <summary>
        /// Scale proportionally and set width to value.
        /// </summary>
        [EnumMember(Value = "WIDTH")]
        Width
    }
}