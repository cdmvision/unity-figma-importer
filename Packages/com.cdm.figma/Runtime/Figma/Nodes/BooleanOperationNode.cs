using System;
using System.Collections.Generic;
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
        public List<Node> children { get; private set; } = new List<Node>();
        
        /// <summary>
        /// Indicates the type of boolean operation applied.
        /// </summary>
        [JsonProperty("booleanOperation")]
        public BooleanOperation operation { get; set; }
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