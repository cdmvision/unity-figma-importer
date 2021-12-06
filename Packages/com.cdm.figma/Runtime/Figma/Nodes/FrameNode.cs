using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The frame node is a container used to define a layout hierarchy. It is similar to <![CDATA[<div>]]> in HTML.
    /// It is different from <see cref="GroupNode"/>, which is closer to a folder for layers.
    /// 
    /// Frames generally have their own size, though the size can be determined by that of its children
    /// in the case of auto-layout frames.
    /// </summary>
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