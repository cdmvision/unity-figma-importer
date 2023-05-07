using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Layout constraint relative to containing Frame.
    /// </summary>
    [DataContract]
    public class LayoutConstraint
    {
        /// <summary>
        /// Vertical constraint of the layout.
        /// </summary>
        [DataMember]
        public Vertical vertical { get; set; }

        /// <summary>
        /// Horizontal constraint of the layout.
        /// </summary>
        [DataMember]
        public Horizontal horizontal { get; set; }
    }

    [DataContract]
    public enum Horizontal
    {
        /// <summary>
        /// Node is horizontally centered relative to containing frame.
        /// </summary>
        [EnumMember(Value = "CENTER")]
        Center,

        /// <summary>
        /// Node is laid out relative to left of the containing frame.
        /// </summary>
        [EnumMember(Value = "LEFT")]
        Left,

        /// <summary>
        /// Both left and right of node are constrained relative to containing frame (node stretches with frame).
        /// </summary>
        [EnumMember(Value = "LEFT_RIGHT")]
        LeftRight,

        /// <summary>
        /// Node is laid out relative to right of the containing frame.
        /// </summary>
        [EnumMember(Value = "RIGHT")]
        Right,

        /// <summary>
        /// Node scales horizontally with containing frame.
        /// </summary>
        [EnumMember(Value = "SCALE")]
        Scale
    }

    [DataContract]
    public enum Vertical
    {
        /// <summary>
        ///  Node is laid out relative to bottom of the containing frame.
        /// </summary>
        [EnumMember(Value = "BOTTOM")]
        Bottom,

        /// <summary>
        /// Node is vertically centered relative to containing frame.
        /// </summary>
        [EnumMember(Value = "CENTER")]
        Center,

        /// <summary>
        /// Node scales vertically with containing frame.
        /// </summary>
        [EnumMember(Value = "SCALE")]
        Scale,

        /// <summary>
        /// Node is laid out relative to top of the containing frame.
        /// </summary>
        [EnumMember(Value = "TOP")]
        Top,

        /// <summary>
        /// Both top and bottom of node are constrained relative to containing frame.
        /// </summary>
        [EnumMember(Value = "TOP_BOTTOM")]
        TopBottom
    }
}