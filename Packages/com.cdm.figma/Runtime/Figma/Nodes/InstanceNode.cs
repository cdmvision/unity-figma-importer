using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class InstanceNode : FrameNode
    {
        public override NodeType type => NodeType.Instance;
        
        /// <summary>
        /// ID of component that this instance came from, refers to components table.
        /// </summary>
        [JsonProperty("componentId", Required = Required.Always)]
        public string componentId { get; set; }
    }
}