using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class BooleanOperationNode : VectorNode
    {
        public override string type => NodeType.Boolean;

        /// <summary>
        /// A list of nodes that are being boolean operated on.
        /// </summary>
        [DataMember(Name = "children")]
        public Node[] children { get; set; }
        
        /// <summary>
        /// Indicates the type of boolean operation applied.
        /// </summary>
        [DataMember(Name = "booleanOperation")]
        public BooleanOperation operation { get; set; }

        public override Node[] GetChildren() => children;
    }

    [DataContract]
    public enum BooleanOperation
    {
        [EnumMember(Value = "UNION")]
        Union,
        
        [EnumMember(Value = "INTERSECT")]
        Intersect,
        
        [EnumMember(Value = "SUBTRACT")]
        Subtract,
        
        [EnumMember(Value = "EXCLUDE")]
        Exclude
    }
}