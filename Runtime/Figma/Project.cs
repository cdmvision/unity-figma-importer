using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class Project
    {
        [DataMember(Name = "id", IsRequired = true)]
        public float id { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }
    }
}