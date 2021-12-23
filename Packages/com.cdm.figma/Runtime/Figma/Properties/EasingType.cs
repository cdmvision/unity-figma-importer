using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Enum describing animation easing curves.
    /// </summary>
    [DataContract]
    public enum EasingType
    {
        /// <summary>
        /// Ease in with an animation curve similar to CSS ease-in.
        /// </summary>
        [EnumMember(Value = "EASE_IN")]
        EaseIn,

        /// <summary>
        /// Ease out with an animation curve similar to CSS ease-out.
        /// </summary>
        [EnumMember(Value = "EASE_OUT")]
        EaseOut,

        /// <summary>
        /// Ease in and then out with an animation curve similar to CSS ease-in-out.
        /// </summary>
        [EnumMember(Value = "EASE_IN_AND_OUT")]
        EaseInAndOut,

        /// <summary>
        /// No easing, similar to CSS linear.
        /// </summary>
        [EnumMember(Value = "LINEAR")]
        Linear,
        
        /// <summary>
        /// Describes the points that define the cubic bezier easing curve (x1, y1, x2, y2).
        /// </summary>
        [EnumMember(Value = "CUSTOM_CUBIC")]
        CustomCubic
    }
}