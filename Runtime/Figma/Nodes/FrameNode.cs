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
    public class FrameNode : GroupNode, INodeFill, INodeRect
    {
        public override string type => NodeType.Frame;
        
        /// <summary>
        /// A list of layout grids attached to this node (see layout grids section for more details).
        /// </summary>
        [DataMember(Name = "layoutGrids")]
        public List<LayoutGrid> layoutGrids { get; private set; } = new List<LayoutGrid>();
        
        [DataMember(Name = "fills")]
        public List<Paint> fills { get; private set; } = new List<Paint>();
        
        [DataMember(Name = "strokes")]
        public List<Paint> strokes { get; private set; } = new List<Paint>();
        
        [DataMember(Name = "strokeWeight")]
        public float? strokeWeight { get; set; }
        
        [DataMember(Name = "strokeAlign")]
        public StrokeAlign? strokeAlign { get; set; }
        
        [DataMember(Name = "cornerRadius")]
        public float? cornerRadius { get; set; }
        
        [DataMember(Name = "rectangleCornerRadii")]
        public float[] rectangleCornerRadii { get; set; }
        
        [DataMember(Name = "blendMode")]
        public BlendMode? blendMode { get; set; }
        
        [DataMember(Name = "opacity")]
        public float opacity { get; set; } = 1f;
        
        /// <summary>
        /// Determines the canvas stacking order of layers in this frame. When <c>true</c>, the first layer will be
        /// draw on top. This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "itemReverseZIndex")]
        public bool itemReverseZIndex { get; set; } = false;
    }
}