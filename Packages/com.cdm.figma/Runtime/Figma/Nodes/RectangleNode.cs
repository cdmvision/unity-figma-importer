using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The rectangle is one of the most commonly used shapes in Figma. A notable feature it has over other kinds of
    /// shapes is the ability to specify independent corner radius values.
    /// </summary>
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

        public float topLeftRadius => GetCornerRadius(0);
        public float topRightRadius => GetCornerRadius(1);
        public float bottomLeftRadius => GetCornerRadius(2);
        public float bottomRightRadius => GetCornerRadius(3);

        private float GetCornerRadius(int i)
        {
            if (rectangleCornerRadii != null)
            {
                return rectangleCornerRadii[i];
            }

            if (cornerRadius.HasValue)
            {
                return cornerRadius.Value;
            }

            return 0;
        }
    }
}