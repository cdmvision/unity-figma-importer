using UnityEngine;

namespace Cdm.Figma
{
    public interface INodeLayout
    {
        /// <summary>
        /// This property is applicable only for direct children of auto-layout frames, ignored otherwise.
        /// Determines whether a layer should stretch along the parent’s primary axis. A 0 corresponds to a fixed size
        /// and 1 corresponds to stretch. See <see cref="LayoutGrow"/>.
        /// </summary>
        public float? layoutGrow { get; set; }

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
        /// Applicable only to auto-layout frames and their direct children.
        /// </summary>
        public float? minWidth { get; set; }

        /// <summary>
        /// Applicable only to auto-layout frames and their direct children.
        /// </summary>
        public float? maxWidth { get; set; }

        /// <summary>
        /// Applicable only to auto-layout frames and their direct children.
        /// </summary>
        public float? minHeight { get; set; }

        /// <summary>
        /// Applicable only to auto-layout frames and their direct children.
        /// </summary>
        public float? maxHeight { get; set; }
    }
    
    public static class LayoutGrow
    {
        public const float Fixed = 0;
        public const float Stretch = 1;

        public static bool Equals(float a, float b)
        {
            return Mathf.Approximately(a, b);
        }

        public static bool IsFixed(float value)
        {
            return Equals(value, Fixed);
        }
        
        public static bool IsStretch(float value)
        {
            return Equals(value, Stretch);
        }
    }
}