using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class RectangleNode : VectorNode
    {
        public override string type => NodeType.Rectangle;
        
        /// <summary>
        /// Radius of each corner of the frame if a single radius is set for all corners.
        /// </summary>
        [DataMember(Name = "cornerRadius")]
        public float? cornerRadius { get; set; }
        
        /// <summary>
        /// Array of length 4 of the radius of each corner of the frame, starting in the top left and
        /// proceeding clockwise.
        /// </summary>
        [DataMember(Name = "rectangleCornerRadii")]
        public float[] rectangleCornerRadii { get; set; }
    }
}