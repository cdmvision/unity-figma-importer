using System;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [Serializable]
    public enum TextAutoResize
    {
        [EnumMember(Value = "NONE")]
        None,
        
        [EnumMember(Value = "HEIGHT")]
        Height,
        
        [EnumMember(Value = "WIDTH_AND_HEIGHT")]
        WidthAndHeight
    }
}