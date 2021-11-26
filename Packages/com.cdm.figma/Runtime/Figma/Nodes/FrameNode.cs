using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class FrameNode : GroupNode
    {
        public override string type => NodeType.Frame;
        
        /// <summary>
        /// A list of layout grids attached to this node (see layout grids section for more details).
        /// </summary>
        [DataMember(Name = "layoutGrids")]
        public List<LayoutGrid> layoutGrids { get; private set; } = new List<LayoutGrid>();
    }
}