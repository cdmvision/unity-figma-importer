using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The rectangle is one of the most commonly used shapes in Figma. A notable feature it has over other kinds of
    /// shapes is the ability to specify independent corner radius values.
    /// </summary>
    [DataContract]
    public class RectangleNode : VectorNode, INodeRect
    {
        public override string type => NodeType.Rectangle;
        
        [DataMember(Name = "cornerRadius")]
        public float? cornerRadius { get; set; }
        
        [DataMember(Name = "rectangleCornerRadii")]
        public float[] rectangleCornerRadii { get; set; }
    }
}