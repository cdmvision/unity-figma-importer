using System;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [Serializable]
    public enum TextCase
    {
        [EnumMember(Value = "ORIGINAL")]
        Original,
        
        [EnumMember(Value = "UPPER")]
        Upper,
        
        [EnumMember(Value = "LOWER")]
        Lower,
        
        [EnumMember(Value = "TITLE")]
        Title,
        
        [EnumMember(Value = "SMALL_CAPS")]
        SmallCaps,
        
        [EnumMember(Value = "SMALL_CAPS_FORCED")]
        SmallCapsForced
    }
}