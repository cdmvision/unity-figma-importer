using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public enum ComponentPropertyType
    {
        [EnumMember(Value = "BOOLEAN")]
        Boolean,

        [EnumMember(Value = "TEXT")]
        Text,

        [EnumMember(Value = "INSTANCE_SWAP")]
        InstanceSwap,

        [EnumMember(Value = "VARIANT")]
        Variant,
    }
}