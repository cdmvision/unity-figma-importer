using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A rectangle that expresses a bounding box in absolute coordinates.
    /// </summary>
    [DataContract]
    public class Rectangle
    {
        /// <summary>
        /// X coordinate of top left corner of the rectangle.
        /// </summary>
        [DataMember(Name = "x")]
        public float x { get; set; }

        /// <summary>
        /// Y coordinate of top left corner of the rectangle.
        /// </summary>
        [DataMember(Name = "y")]
        public float y { get; set; }
        
        /// <summary>
        /// Width of the rectangle
        /// </summary>
        [DataMember(Name = "width")]
        public float width { get; set; }
        
        /// <summary>
        /// Height of the rectangle.
        /// </summary>
        [DataMember(Name = "height")]
        public float height { get; set; }
    }
}