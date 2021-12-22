namespace Cdm.Figma
{
    public interface INodeLayout
    {
        /// <summary>
        /// This property is applicable only for direct children of auto-layout frames, ignored otherwise.
        /// Determines whether a layer should stretch along the parent’s primary axis. A 0 corresponds to a fixed size
        /// and 1 corresponds to stretch.
        /// </summary>
        public int? layoutGrow { get; set; }
        
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