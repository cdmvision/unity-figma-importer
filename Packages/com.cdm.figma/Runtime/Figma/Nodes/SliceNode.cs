using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class SliceNode : Node
    {
        public override string type => NodeType.Slice;
        
        /// <summary>
        /// A list of export settings representing images to export from the canvas.
        /// </summary>
        [DataMember(Name = "exportSettings")]
        public List<ExportSetting> exportSettings { get; private set; } = new List<ExportSetting>(); 
        
        /// <summary>
        /// Bounding box of the node in absolute space coordinates.
        /// </summary>
        [DataMember(Name = "absoluteBoundingBox")]
        public Rectangle absoluteBoundingBox { get; set; }
        
        /// <summary>
        /// Width and height of element. This is different from the width and height of the bounding box in that the
        /// absolute bounding box represents the element after scaling and rotation.
        ///
        /// Only present if geometry=paths is passed.
        /// </summary>
        [DataMember(Name = "size")]
        public Vector size { get; set; }
        
        /// <summary>
        /// The top two rows of a matrix that represents the 2D transform of this node relative to its parent.
        /// The bottom row of the matrix is implicitly always (0, 0, 1). Use to transform coordinates in geometry.
        /// Only present if geometry=paths is passed.
        /// </summary>
        [DataMember(Name = "relativeTransform")]
        public AffineTransform relativeTransform { get; set; }
    }
}