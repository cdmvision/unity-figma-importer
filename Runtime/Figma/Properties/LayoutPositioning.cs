using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Determines whether a layer's size and position should be determined by auto-layout settings or
    /// manually adjustable.
    /// </summary>
    [DataContract]
    public enum LayoutPositioning
    {
        /// <summary>
        /// It will layout this child according to auto-layout rules.
        /// </summary>
        [EnumMember(Value = "AUTO")]
        Auto,
        
        /// <summary>
        /// This allows explicitly setting x, y, width, and height. <see cref="Absolute"/> positioned nodes respect
        /// constraint settings.
        /// </summary>
        [EnumMember(Value = "ABSOLUTE")]
        Absolute
    }
}