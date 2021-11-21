using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// Metadata for character formatting.
    /// 
    /// Style of text including font family and weight (see type style
    /// section for more information).
    /// Map from ID to TypeStyle for looking up style overrides.
    /// </summary>
    [Serializable]
    public partial class TypeStyle
    {
        /// <summary>
        /// Font family of text (standard name)
        /// </summary>
        [JsonProperty("fontFamily")]
        public string fontFamily { get; set; }

        /// <summary>
        /// PostScript font name
        /// </summary>
        [JsonProperty("fontPostScriptName")]
        public string fontPostScriptName { get; set; }

        /// <summary>
        /// Space between paragraphs in px, 0 if not present.
        /// </summary>
        [JsonProperty("paragraphSpacing")]
        public float paragraphSpacing { get; set; }

        /// <summary>
        /// Paragraph indentation in px, 0 if not present.
        /// </summary>
        [JsonProperty("paragraphIndent")]
        public float paragraphIndent { get; set; }

        /// <summary>
        /// Space between list items in px, 0 if not present.
        /// </summary>
        [JsonProperty("listSpacing")]
        public float listSpacing { get; set; }

        /// <summary>
        /// Whether or not text is italicized.
        /// </summary>
        [JsonProperty("italic")]
        public bool italic { get; set; }

        /// <summary>
        /// Numeric font weight.
        /// </summary>
        [JsonProperty("fontWeight")]
        public float fontWeight { get; set; }

        /// <summary>
        /// Font size in px.
        /// </summary>
        [JsonProperty("fontSize")]
        public float fontSize { get; set; }

        /// <summary>
        /// Text casing applied to the node, default is the <see cref="TextCase.Original"/> casing.
        /// </summary>
        [JsonProperty("textCase")]
        public TextCase textCase { get; set; } = TextCase.Original;

        /// <summary>
        /// Text decoration applied to the node, default is <see cref="TextDecoration.None"/>.
        /// </summary>
        [JsonProperty("textDecoration")]
        public TextDecoration textDecoration { get; set; } = TextDecoration.None;

        /// <summary>
        /// Dimensions along which text will auto resize, default is that the text does not auto-resize.
        /// </summary>
        [JsonProperty("textAutoResize")]
        public TextAutoResize textAutoResize { get; set; } = TextAutoResize.None;

        /// <summary>
        /// Horizontal text alignment as string enum
        /// </summary>
        [JsonProperty("textAlignHorizontal")]
        public TextAlignHorizontal textAlignHorizontal { get; set; } = TextAlignHorizontal.Left;

        /// <summary>
        /// Vertical text alignment as string enum
        /// </summary>
        [JsonProperty("textAlignVertical")]
        public TextAlignVertical textAlignVertical { get; set; } = TextAlignVertical.Top;

        /// <summary>
        /// Space between characters in px
        /// </summary>
        [JsonProperty("letterSpacing")]
        public float letterSpacing { get; set; }

        /// <summary>
        /// Paints applied to characters.
        /// </summary>
        [JsonProperty("fills")]
        public List<Paint> fills { get; private set; } = new List<Paint>();

        /// <summary>
        /// Link to a URL or frame.
        /// </summary>
        [JsonProperty("hyperlink")]
        public Hyperlink hyperlink { get; set; }

        /// <summary>
        /// A map of OpenType feature flags to 1 or 0, 1 if it is enabled and 0 if it is disabled.
        /// Note that some flags aren't reflected here. For example, SMCP (small caps) is still represented
        /// by the <see cref="textCase"/> field.
        /// </summary>
        [JsonProperty("opentypeFlags")]
        public Dictionary<string, int> opentypeFlags { get; private set; } = new Dictionary<string, int>();

        /// <summary>
        /// Line height in px.
        /// </summary>
        [JsonProperty("lineHeightPx")]
        public float lineHeightPx { get; set; }

        /// <summary>
        /// Line height as a percentage of the font size. Only returned when lineHeightPercent is not 100.
        /// </summary>
        [JsonProperty("lineHeightPercentFontSize")]
        public float lineHeightPercentFontSize { get; set; } = 100;

        /// <summary>
        /// The unit of the line height value specified by the user.
        /// </summary>
        /// <list type="bullet">
        /// <item>PIXELS</item>
        /// <item>FONT_SIZE_%</item>
        /// <item>INTRINSIC_%</item>
        /// </list>
        [JsonProperty("lineHeightUnit")]
        public string lineHeightUnit { get; set; }
    }

    [Serializable]
    public enum TextAlignHorizontal
    {
        [EnumMember(Value = "CENTER")]
        Center,

        [EnumMember(Value = "JUSTIFIED")]
        Justified,

        [EnumMember(Value = "LEFT")]
        Left,

        [EnumMember(Value = "RIGHT")]
        Right
    }

    [Serializable]
    public enum TextAlignVertical
    {
        [EnumMember(Value = "BOTTOM")]
        Bottom,

        [EnumMember(Value = "CENTER")]
        Center,

        [EnumMember(Value = "TOP")]
        Top
    }
}