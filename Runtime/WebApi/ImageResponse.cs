using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ImageResponse : BaseResponse
    {
        [DataMember(Name = "images")]
        public Dictionary<string, string> images { get; set; } = new Dictionary<string, string>();
    }
}