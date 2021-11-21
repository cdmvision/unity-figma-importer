using System;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [Serializable]
    public enum TextDecoration
    {
        [EnumMember(Value = "NONE")]
        None,
        
        [EnumMember(Value = "STRIKETHROUGH")]
        Strikethrough,
        
        [EnumMember(Value = "UNDERLINE")]
        Underline
    }
}