namespace Cdm.Figma
{
    public interface INodeLayout
    {
        /// <summary>
        /// Horizontal and vertical layout constraints for node.
        /// </summary>
        public LayoutConstraint constraints { get; set; }
        
        /// <summary>
        /// Determines if the layer should stretch along the parent’s counter axis. This property is only provided for
        /// direct children of auto-layout frames.
        /// </summary>
        public LayoutAlign layoutAlign { get; set; }
    }
}