using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A slice is an invisible object with a bounding box, represented as dashed lines in the editor. Its purpose
    /// is to allow you to export a specific part of a document. Generally, the only thing you will do with a slice
    /// is to add an <see cref="exportSettings"/>.
    /// </summary>
    [DataContract]
    public class SliceNode : SceneNode
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