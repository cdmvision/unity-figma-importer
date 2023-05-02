using System;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentMetadata
    {
        [DataMember(Name = "key", IsRequired = true)]
        public string key { get; set; }

        [DataMember(Name = "file_key", IsRequired = true)]
        public string fileKey { get; set; }

        [DataMember(Name = "node_id", IsRequired = true)]
        public string nodeId { get; set; }

        [DataMember(Name = "thumbnail_url")]
        public string thumbnailUrl { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "description")]
        public string description { get; set; }

        [DataMember(Name = "created_at")]
        public DateTime createdAt { get; set; }

        [DataMember(Name = "updated_at")]
        public DateTime updatedAt { get; set; }

        [DataMember(Name = "user")]
        public User user { get; set; }
    }
}