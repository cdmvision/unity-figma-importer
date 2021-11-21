using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Positioning of the grid.
    /// </summary>
    [DataContract]
    public enum Alignment
    {
        /// <summary>
        /// Grid is center aligned.
        /// </summary>
        [EnumMember(Value = "CENTER")]
        Center,

        /// <summary>
        /// Grid starts at the right or bottom of the frame.
        /// </summary>
        [EnumMember(Value = "MAX")]
        Max,

        /// <summary>
        /// Grid starts at the left or top of the frame.
        /// </summary>
        [EnumMember(Value = "MIN")]
        Min,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "STRETCH")]
        Stretch
    };
}