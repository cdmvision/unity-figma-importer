using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ImageFillsResponse : BaseResponse
    {
        [DataMember(Name = "meta")]
        public ImageFillsMetadata metadata { get; set; }
    }

    [DataContract]
    public class ImageFillsMetadata
    {
        [DataMember(Name = "images")]
        public Dictionary<string, string> images { get; set; } = new Dictionary<string, string>();
    }
}