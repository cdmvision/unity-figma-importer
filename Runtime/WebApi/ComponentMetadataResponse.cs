using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentMetadataResponse : BaseResponse
    {
        [DataMember(Name = "meta")]
        public ComponentMetadata metadata { get; set; }
    }
}