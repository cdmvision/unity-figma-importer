namespace Cdm.Figma
{
    public interface INodeLayout
    {
        /// <summary>
        /// Width and height of element. This is different from the width and height of the bounding box in that the
        /// absolute bounding box represents the element after scaling and rotation.
        /// </summary>
        public Vector size { get; set; }
        
        /// <summary>
        /// Horizontal and vertical layout constraints for node.
        /// </summary>
        public LayoutConstraint constraints { get; set; }
        
        /// <summary>
        /// Determines if the layer should stretch along the parent’s counter axis. This property is only provided for
        /// direct children of auto-layout frames.
        /// </summary>
        public LayoutAlign layoutAlign { get; set; }
        
        /// <summary>
        /// Bounding box of the node in absolute space coordinates.
        /// </summary>
        public Rectangle absoluteBoundingBox { get; set; }
        
        /// <summary>
        /// The top two rows of a matrix that represents the 2D transform of this node relative to its parent.
        /// The bottom row of the matrix is implicitly always (0, 0, 1). Use to transform coordinates in geometry.
        /// Only present if geometry=paths is passed.
        /// </summary>
        public AffineTransform relativeTransform { get; set; }
    }
}