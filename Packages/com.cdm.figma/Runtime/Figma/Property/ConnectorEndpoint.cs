using System;
using System.Runtime.Serialization;
using Unity.Plastic.Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// Stores canvas location for a connector start/end point.
    /// </summary>
    [Serializable]
    public class ConnectorEndpoint
    {
        /// <summary>
        /// Node ID this endpoint attaches to.
        /// </summary>
        [JsonProperty("endpointNodeId")]
        public string endpointNodeId { get; set; }
        
        #region ConnectorEndpoint with endpointNodeId and position only:

        /// <summary>
        /// Canvas location as x & y coordinate.
        /// </summary>
        [JsonProperty("position")]
        public Vector position { get; set; }

        #endregion

        #region ConnectorEndpoint with endpointNodeId and magnet only:

        /// <summary>
        /// The magnet type.
        /// </summary>
        [JsonProperty("magnet")]
        public ConnectorMagnet magnet { get; set; } = ConnectorMagnet.Auto;

        #endregion
    }

    [Serializable]
    public enum ConnectorMagnet
    {
        [EnumMember(Value = "AUTO")]
        Auto,

        [EnumMember(Value = "TOP")]
        Top,

        [EnumMember(Value = "BOTTOM")]
        Bottom,

        [EnumMember(Value = "LEFT")]
        Left,

        [EnumMember(Value = "RIGHT")]
        Right
    }

    [Serializable]
    public enum ConnectorLineType
    {
        [EnumMember(Value = "ELBOWED")]
        Elbowed,
        
        [EnumMember(Value = "STRAIGHT")]
        Straight
    }
}