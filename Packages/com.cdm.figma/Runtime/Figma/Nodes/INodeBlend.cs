using System.Collections.Generic;

namespace Cdm.Figma
{
    public interface INodeBlend
    {
        /// <summary>
        /// Opacity of the node.
        /// </summary>
        public float opacity { get; set; }

        /// <summary>
        /// Does this node mask sibling nodes in front of it?
        /// </summary>
        public bool isMask { get; set; }
        
        /// <summary>
        /// How this node blends with nodes behind it in the scene.
        /// </summary>
        public BlendMode? blendMode { get; set; }

        /// <summary>
        /// An array of fill paints applied to the node.
        /// </summary>
        public List<Paint> fills { get; }
        
        /// <summary>
        /// An array of stroke paints applied to the node.
        /// </summary>
        public List<Paint> strokes { get; }
        
        /// <summary>
        /// The weight of strokes on the node.
        /// </summary>
        public float? strokeWeight { get; set; }

        /// <summary>
        /// Position of stroke relative to vector outline.
        /// </summary>
        public StrokeAlign? strokeAlign { get; set; }
        
        /// <summary>
        /// A list of effects attached to this node.
        /// </summary>
        public List<Effect> effects { get; }
    }
}