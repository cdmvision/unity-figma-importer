using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class BooleanOperationNode : VectorNode
    {
        public override NodeType type => NodeType.Boolean;

        /// <summary>
        /// A list of nodes that are being boolean operated on.
        /// </summary>
        [JsonProperty("children")]
        public Node[] children { get; set; }
        
        /// <summary>
        /// Indicates the type of boolean operation applied.
        /// </summary>
        [JsonProperty("booleanOperation")]
        public BooleanOperation operation { get; set; }

        public override Node[] GetChildren() => children;
    }

    [Serializable]
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