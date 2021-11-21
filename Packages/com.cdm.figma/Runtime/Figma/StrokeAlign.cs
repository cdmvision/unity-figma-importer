using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Where stroke is drawn relative to the vector outline.
    /// </summary>
    [DataContract]
    public enum StrokeAlign
    {
        /// <summary>
        /// Draw stroke centered along the shape boundary.
        /// </summary>
        [EnumMember(Value = "CENTER")]
        Center,

        /// <summary>
        /// Draw stroke inside the shape boundary.
        /// </summary>
        [EnumMember(Value = "INSIDE")]
        Inside,

        /// <summary>
        /// Draw stroke outside the shape boundary.
        /// </summary>
        [EnumMember(Value = "OUTSIDE")]
        Outside
    };
}