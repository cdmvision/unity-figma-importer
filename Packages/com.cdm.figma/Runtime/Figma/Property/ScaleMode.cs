using System;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [Serializable]
    public enum ScaleMode
    {
        [EnumMember(Value = "FILL")]
        Fill,
        
        [EnumMember(Value = "FIT")]
        Fit,
        
        [EnumMember(Value = "TILE")]
        Tile,
        
        [EnumMember(Value = "STRETCH")]
        Stretch
    }
}