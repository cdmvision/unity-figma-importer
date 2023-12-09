using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class GradientPaint : Paint
    {
        /// <summary>
        /// This field contains three vectors, each of which are a position in
        /// normalized object space (normalized object space is if the top left
        /// corner of the bounding box of the object is (0, 0) and the bottom
        /// right is (1,1)). The first position corresponds to the start of the
        /// gradient (value 0 for the purposes of calculating gradient stops),
        /// the second position is the end of the gradient (value 1), and the
        /// third handle position determines the width of the gradient (only
        /// relevant for non-linear gradients).
        /// </summary>
        [DataMember(Name = "gradientHandlePositions")]
        public Vector[] gradientHandlePositions { get; private set; }

        /// <summary>
        /// Positions of key points along the gradient axis with the colors
        /// anchored there. Colors along the gradient are interpolated smoothly
        /// between neighboring gradient stops.
        /// </summary>
        [DataMember(Name = "gradientStops")]
        public ColorStop[] gradientStops { get; private set; }
    }
    
    /// <summary>
    /// A position color pair representing a gradient stop.
    ///
    /// Positions of key points along the gradient axis with the colors
    /// anchored there. Colors along the gradient are interpolated smoothly
    /// between neighboring gradient stops.
    /// </summary>
    [DataContract]
    public class ColorStop
    {
        /// <summary>
        /// Value between 0 and 1 representing position along gradient axis.
        /// </summary>
        [DataMember(Name = "position")]
        public float position { get; set; }
        
        /// <summary>
        /// Color attached to corresponding position.
        /// </summary>
        [DataMember(Name = "color")]
        public Color color { get; set; }
    }
}