using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Metadata for character formatting.
    /// 
    /// Style of text including font family and weight (see type style
    /// section for more information).
    /// Map from ID to TypeStyle for looking up style overrides.
    /// </summary>
    [DataContract]
    public class TypeStyle
    {
        /// <summary>
        /// Font family of text (standard name)
        /// </summary>
        [DataMember(Name = "fontFamily")]
        public string fontFamily { get; set; }

        /// <summary>
        /// PostScript font name
        /// </summary>
        [DataMember(Name = "fontPostScriptName")]
        public string fontPostScriptName { get; set; }

        /// <summary>
        /// Space between paragraphs in px, 0 if not present.
        /// </summary>
        [DataMember(Name = "paragraphSpacing")]
        public float paragraphSpacing { get; set; }

        /// <summary>
        /// Paragraph indentation in px, 0 if not present.
        /// </summary>
        [DataMember(Name = "paragraphIndent")]
        public float paragraphIndent { get; set; }

        /// <summary>
        /// Space between list items in px, 0 if not present.
        /// </summary>
        [DataMember(Name = "listSpacing")]
        public float listSpacing { get; set; }

        /// <summary>
        /// Whether or not text is italicized.
        /// </summary>
        [DataMember(Name = "italic")]
        public bool italic { get; set; }

        /// <summary>
        /// Numeric font weight.
        /// </summary>
        [DataMember(Name = "fontWeight")]
        public int fontWeight { get; set; }

        /// <summary>
        /// Font size in px.
        /// </summary>
        [DataMember(Name = "fontSize")]
        public float fontSize { get; set; }

        /// <summary>
        /// Text casing applied to the node, default is the <see cref="TextCase.Original"/> casing.
        /// </summary>
        [DataMember(Name = "textCase")]
        public TextCase textCase { get; set; } = TextCase.Original;

        /// <summary>
        /// Text decoration applied to the node, default is <see cref="TextDecoration.None"/>.
        /// </summary>
        [DataMember(Name = "textDecoration")]
        public TextDecoration textDecoration { get; set; } = TextDecoration.None;

        /// <summary>
        /// Dimensions along which text will auto resize, default is that the text does not auto-resize.
        /// </summary>
        [DataMember(Name = "textAutoResize")]
        public TextAutoResize textAutoResize { get; set; } = TextAutoResize.None;

        /// <summary>
        /// Horizontal text alignment as string enum
        /// </summary>
        [DataMember(Name = "textAlignHorizontal")]
        public TextAlignHorizontal textAlignHorizontal { get; set; } = TextAlignHorizontal.Left;

        /// <summary>
        /// Vertical text alignment as string enum
        /// </summary>
        [DataMember(Name = "textAlignVertical")]
        public TextAlignVertical textAlignVertical { get; set; } = TextAlignVertical.Top;

        /// <summary>
        /// Space between characters in px
        /// </summary>
        [DataMember(Name = "letterSpacing")]
        public float letterSpacing { get; set; }

        /// <summary>
        /// Paints applied to characters.
        /// </summary>
        [DataMember(Name = "fills")]
        public List<Paint> fills { get; private set; } = new List<Paint>();

        /// <summary>
        /// Link to a URL or frame.
        /// </summary>
        [DataMember(Name = "hyperlink")]
        public Hyperlink hyperlink { get; set; }

        /// <summary>
        /// A map of OpenType feature flags to 1 or 0, 1 if it is enabled and 0 if it is disabled.
        /// Note that some flags aren't reflected here. For example, SMCP (small caps) is still represented
        /// by the <see cref="textCase"/> field.
        /// </summary>
        [DataMember(Name = "opentypeFlags")]
        public Dictionary<string, int> opentypeFlags { get; private set; } = new Dictionary<string, int>();

        /// <summary>
        /// Line height in px.
        /// </summary>
        [DataMember(Name = "lineHeightPx")]
        public float lineHeightPx { get; set; }

        /// <summary>
        /// Line height as a percentage of the font size. Only returned when lineHeightPercent is not 100.
        /// </summary>
        [DataMember(Name = "lineHeightPercentFontSize")]
        public float lineHeightPercentFontSize { get; set; } = 100;

        /// <summary>
        /// The unit of the line height value specified by the user.
        /// </summary>
        /// <list type="bullet">
        /// <item>PIXELS</item>
        /// <item>FONT_SIZE_%</item>
        /// <item>INTRINSIC_%</item>
        /// </list>
        [DataMember(Name = "lineHeightUnit")]
        public string lineHeightUnit { get; set; }

        /// <summary>
        /// Whether this text node will truncate with an ellipsis when the text node size is smaller than the
        /// text inside.
        /// </summary>
        [DataMember(Name = "textTruncation")]
        public TextTruncation? textTruncation { get; set; }
        
        /// <summary>
        /// The maximum number of lines a text node can reach before it truncates. Only applicable when
        /// <see cref="textTruncation"/> is set to <see cref="TextTruncation.Ending"/>.
        /// </summary>
        [DataMember(Name = "maxLines")]
        public int? maxLines { get; set; }
    }

    /// <summary>
    /// The horizontal alignment of the text with respect to the text box.
    /// </summary>
    [DataContract]
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
    
    /// <summary>
    /// The vertical alignment of the text with respect to the text box.
    /// </summary>
    [DataContract]
    public enum TextAlignVertical
    {
        [EnumMember(Value = "BOTTOM")]
        Bottom,

        [EnumMember(Value = "CENTER")]
        Center,

        [EnumMember(Value = "TOP")]
        Top
    }
    
    /// <summary>
    /// The behavior of how the size of the text box adjusts to fit the characters.
    /// </summary>
    [DataContract]
    public enum TextAutoResize
    {
        /// <summary>
        /// The size of the text box is fixed and is independent of its content.
        /// </summary>
        [EnumMember(Value = "NONE")]
        None,
        
        /// <summary>
        /// The width of the text box is fixed. Characters wrap to fit in the text box. The height of the text box
        /// automatically adjusts to fit its content.
        /// </summary>
        [EnumMember(Value = "HEIGHT")]
        Height,
        
        /// <summary>
        /// Both the width and height of the text box automatically adjusts to fit its content. Characters do not wrap.
        /// </summary>
        [EnumMember(Value = "WIDTH_AND_HEIGHT")]
        WidthAndHeight,
        
        /// <summary>
        /// Like <see cref="None"/>, but text that overflows the bounds of the text node will be truncated with an
        /// ellipsis. This value will be removed in the future - prefer reading from textTruncation instead.
        /// </summary>
        [Obsolete("Prefer reading from textTruncation instead.")]
        [EnumMember(Value = "TRUNCATE")]
        Truncate
    }

    /// <summary>
    /// Whether this text node will truncate with an ellipsis when the text node size is smaller than the text inside.
    /// </summary>
    /// <remarks>
    /// When <see cref="TypeStyle.textAutoResize"/> is set to <see cref="TextAutoResize.None"/>, the text will truncate
    /// when the fixed size is smaller than the text inside. When it is <see cref="TextAutoResize.Height"/> or
    /// <see cref="TextAutoResize.WidthAndHeight"/>, truncation will  only occur if used in conjunction with
    /// maxHeight or maxLines.
    /// </remarks>
    [DataContract]
    public enum TextTruncation
    {
        /// <summary>
        /// No truncation will occur.
        /// </summary>
        [EnumMember(Value = "DISABLED")]
        Disabled,
        
        /// <summary>
        /// Text node will truncate with an ellipsis when the text node size is smaller than the text inside.
        /// </summary>
        [EnumMember(Value = "ENDING")]
        Ending,
    }
    
    [DataContract]
    public enum TextCase
    {
        /// <summary>
        /// Show the text as defined, no overrides.
        /// </summary>
        [EnumMember(Value = "ORIGINAL")]
        Original,
        
        /// <summary>
        /// All characters are in upper case.
        /// </summary>
        [EnumMember(Value = "UPPER")]
        Upper,
        
        /// <summary>
        /// All characters are in lower case.
        /// </summary>
        [EnumMember(Value = "LOWER")]
        Lower,
        
        /// <summary>
        /// The first character of each word is upper case and all other characters are in lower case.
        /// </summary>
        [EnumMember(Value = "TITLE")]
        Title,
        
        /// <summary>
        /// All characters are in small upper case.
        /// </summary>
        [EnumMember(Value = "SMALL_CAPS")]
        SmallCaps,
        
        /// <summary>
        /// The first character of each word is upper case and all other characters are in small upper case.
        /// </summary>
        [EnumMember(Value = "SMALL_CAPS_FORCED")]
        SmallCapsForced
    }
    
    [DataContract]
    public enum TextDecoration
    {
        /// <summary>
        /// The text is shown without decorations.
        /// </summary>
        [EnumMember(Value = "NONE")]
        None,
        
        /// <summary>
        /// The text has a horizontal line crossing it in the middle.
        /// </summary>
        [EnumMember(Value = "STRIKETHROUGH")]
        Strikethrough,
        
        /// <summary>
        /// The text has a horizontal line underneath it.
        /// </summary>
        [EnumMember(Value = "UNDERLINE")]
        Underline
    }
    
    /// <summary>
    /// A link to either a URL or another frame (node) in the document.
    /// </summary>
    [DataContract]
    public class Hyperlink
    {
        /// <summary>
        /// Type of hyperlink.
        /// </summary>
        [DataMember(Name = "type")]
        public HyperlinkType type { get; set; }
        
        /// <summary>
        /// URL being linked to, if <see cref="HyperlinkType.Url"/> type.
        /// </summary>
        [DataMember(Name = "url")]
        public string url { get; set; }
        
        /// <summary>
        /// ID of frame hyperlink points to, if <see cref="HyperlinkType.Node"/> type
        /// </summary>
        [DataMember(Name = "nodeID")]
        public string nodeId { get; set; }
    }

    [DataContract]
    public enum HyperlinkType
    {
        [EnumMember(Value = "URL")]
        Url,
        
        [EnumMember(Value = "NODE")]
        Node
    }
}