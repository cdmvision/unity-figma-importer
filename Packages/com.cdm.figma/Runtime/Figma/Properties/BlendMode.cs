using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Blend mode describes how a color blends with what's underneath it. This property is typically set on a layer,
    /// fill or effect (e.g. blend mode of the shadow).
    ///
    /// These blend modes are fairly standard and should match what you would find in other image processing tools.
    /// </summary>
    /// <seealso aref="https://developer.mozilla.org/en-US/docs/Web/CSS/blend-mode"/>
    [DataContract]
    public enum BlendMode
    {
        [EnumMember(Value = "COLOR")]
        Color,

        [EnumMember(Value = "COLOR_BURN")]
        ColorBurn,

        [EnumMember(Value = "COLOR_DODGE")]
        ColorDodge,

        [EnumMember(Value = "DARKEN")]
        Darken,

        [EnumMember(Value = "DIFFERENCE")]
        Difference,

        [EnumMember(Value = "EXCLUSION")]
        Exclusion,

        [EnumMember(Value = "HARD_LIGHT")]
        HardLight,

        [EnumMember(Value = "HUE")]
        Hue,

        [EnumMember(Value = "LIGHTEN")]
        Lighten,

        [EnumMember(Value = "LINEAR_BURN")]
        LinearBurn,

        [EnumMember(Value = "LINEAR_DODGE")]
        LinearDodge,

        [EnumMember(Value = "LUMINOSITY")]
        Luminosity,

        [EnumMember(Value = "MULTIPLY")]
        Multiply,

        [EnumMember(Value = "NORMAL")]
        Normal,

        [EnumMember(Value = "OVERLAY")]
        Overlay,

        [EnumMember(Value = "PASS_THROUGH")]
        PassThrough,

        [EnumMember(Value = "SATURATION")]
        Saturation,

        [EnumMember(Value = "SCREEN")]
        Screen,

        [EnumMember(Value = "SOFT_LIGHT")]
        SoftLight
    }
}