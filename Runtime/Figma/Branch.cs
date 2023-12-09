using System;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class Branch
    {
        [DataMember(Name = "key", IsRequired = true)]
        public string key { get; set; }
        
        [DataMember(Name = "name")]
        public string name { get; set; }
        
        [DataMember(Name = "thumbnail_url")]
        public string thumbnailUrl { get; set; }
        
        [DataMember(Name = "last_modified")]
        public DateTime lastModified { get; set; }
    }
}