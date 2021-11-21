using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// How this node blends with nodes behind it in the scene (see blend mode section for more details).
    /// </summary>
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