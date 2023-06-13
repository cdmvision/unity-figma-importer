using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class VectorPath
    {
        [DataMember(Name = "path")]
        public string path { get; set; }

        [DataMember(Name = "overrideID")]
        public int? overrideId { get; set; }
            
        [DataMember(Name = "windingRule")]
        public WindingRule? windingRule { get; set; }
    }
    
    [DataContract]
    public enum WindingRule
    {
        [EnumMember(Value = "NONE")]
        None,
            
        [EnumMember(Value = "NONZERO")]
        NonZero,
            
        [EnumMember(Value = "EVENODD")]
        EvenOdd
    }
}