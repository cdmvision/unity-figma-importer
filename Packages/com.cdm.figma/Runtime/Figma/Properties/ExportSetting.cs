using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// An array of export settings representing images to export from node
    ///
    /// Format and size to export an asset at
    ///
    /// An array of export settings representing images to export from this node
    ///
    /// An array of export settings representing images to export from the canvas
    /// </summary>
    [Serializable]
    public partial class ExportSetting
    {
        /// <summary>
        /// Constraint that determines sizing of exported asset.
        /// </summary>
        [JsonProperty("constraint")]
        public Constraint constraint { get; set; }

        /// <summary>
        /// Image type.
        /// </summary>
        [JsonProperty("format")]
        public ImageFormat format { get; set; }

        /// <summary>
        /// File suffix to append to all filenames.
        /// </summary>
        [JsonProperty("suffix")]
        public string suffix { get; set; }
    }
    
    /// <summary>
    /// Sizing constraint for exports.
    /// </summary>
    [Serializable]
    public class Constraint
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
    
    /// <summary>
    /// Image format.
    /// </summary>
    [Serializable]
    public enum ImageFormat
    {
        [EnumMember(Value = "JPG")]
        Jpg,

        [EnumMember(Value = "PNG")]
        Png,

        [EnumMember(Value = "SVG")]
        Svg
    }
}