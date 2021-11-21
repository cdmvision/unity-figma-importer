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