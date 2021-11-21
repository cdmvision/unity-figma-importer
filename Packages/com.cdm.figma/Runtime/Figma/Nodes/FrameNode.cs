using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class FrameNode : GroupNode
    {
        public override NodeType type => NodeType.Frame;
        
        /// <summary>
        /// A list of layout grids attached to this node (see layout grids section for more details).
        /// </summary>
        [JsonProperty("layoutGrids")]
        public List<LayoutGrid> layoutGrids { get; private set; } = new List<LayoutGrid>();
    }
}