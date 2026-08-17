namespace Cdm.Figma
{
    public interface INodeRect
    {
        /// <summary>
        /// Radius of each corner of the frame if a single radius is set for all corners.
        /// </summary>
        public float? cornerRadius { get; set; }
        
        /// <summary>
        /// Array of length 4 of the radius of each corner of the frame, starting in the top left and
        /// proceeding clockwise.
        /// </summary>
        public float[] rectangleCornerRadii { get; set; }
        
        public float topLeftRadius => GetCornerRadius(0);
        public float topRightRadius => GetCornerRadius(1);
        public float bottomRightRadius => GetCornerRadius(2);
        public float bottomLeftRadius => GetCornerRadius(3);

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