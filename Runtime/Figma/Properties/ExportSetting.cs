using System.Runtime.Serialization;
using Cdm.Figma.Json;
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
    [DataContract]
    public class ExportSetting
    {
        /// <summary>
        /// Constraint that determines sizing of exported asset.
        /// </summary>
        [DataMember(Name = "constraint")]
        public Constraint constraint { get; set; }

        /// <summary>
        /// Image type.
        /// </summary>
        [DataMember(Name = "format")]
        public ExportImageFormat format { get; set; }

        /// <summary>
        /// File suffix to append to all filenames.
        /// </summary>
        [DataMember(Name = "suffix")]
        public string suffix { get; set; }
    }
    
    /// <summary>
    /// Sizing constraint for exports.
    /// </summary>
    [DataContract]
    public class Constraint
    {
        /// <summary>
        /// Type of constraint to apply.
        /// </summary>
        [DataMember(Name = "type")]
        public ConstraintType type { get; set; }

        /// <summary>
        /// See type property for effect of this field.
        /// </summary>
        [DataMember(Name = "value")]
        public float value { get; set; }
    }
    
    /// <summary>
    /// Type of constraint to apply.
    /// </summary>
    [DataContract]
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
    [DataContract]
    [JsonConverter(typeof(DefaultUnknownEnumConverter), Unknown)]
    public enum ExportImageFormat
    {
        Unknown,
        
        [EnumMember(Value = "JPG")]
        Jpg,

        [EnumMember(Value = "PNG")]
        Png,

        [EnumMember(Value = "SVG")]
        Svg,
        
        [EnumMember(Value = "PDF")]
        Pdf
    }
}