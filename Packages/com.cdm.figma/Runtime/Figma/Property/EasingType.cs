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
    }
}