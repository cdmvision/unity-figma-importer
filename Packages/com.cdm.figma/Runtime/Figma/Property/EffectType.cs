using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Type of effect as a string enum
    /// </summary>
    [DataContract]
    public enum EffectType
    {
        [EnumMember(Value = "INNER_SHADOW")]
        InnerShadow,
        
        [EnumMember(Value = "DROP_SHADOW")]
        DropShadow,

        [EnumMember(Value = "LAYER_BLUR")]
        LayerBlur,
        
        [EnumMember(Value = "BACKGROUND_BLUR")]
        BackgroundBlur,
    };
}