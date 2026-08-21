using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Boolean operations combine any set of shape layers through one of four formulas:
    /// <see cref="BooleanOperation.Union"/>, <see cref="BooleanOperation.Subtract"/>,
    /// <see cref="BooleanOperation.Intersect"/>, and <see cref="BooleanOperation.Exclude"/>. The layers to be
    /// combined are stored in its children array.
    /// 
    /// Like the group node, the boolean operations node is always set to fit its children. As such,
    /// its position and size can change when you add or resize its children.
    /// </summary>
    /// <seealso aref="https://help.figma.com/article/65-boolean-operations"/>
    [DataContract]
    public class BooleanNode : VectorNode
    {
        public override NodeType type => NodeType.Boolean;

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