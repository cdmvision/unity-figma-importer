using System.Collections.Generic;

namespace Cdm.Figma
{
    public interface INodeFill
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
        /// An array of floating point numbers describing the pattern of dash length and gap lengths that the vector
        /// path follows. For example a value of [1, 2] indicates that the path has a dash of length 1 followed
        /// by a gap of length 2, repeated.
        /// </summary>
        public float[] strokeDashes { get; set; }
        
        /// <summary>
        /// Only valid if <see cref="strokeJoin"/> is <see cref="StrokeJoin.Miter"/>. The corner angle, in degrees,
        /// below which <see cref="strokeJoin"/>  will be set to <see cref="StrokeJoin.Bevel"/> to avoid super
        /// sharp corners. By default this is 28.96 degrees.
        /// </summary>
        public float strokeMiterAngle { get; set; }
        
        /// <summary>
        /// The stroke join type.
        /// </summary>
        public StrokeJoin strokeJoin { get; set; }
        
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