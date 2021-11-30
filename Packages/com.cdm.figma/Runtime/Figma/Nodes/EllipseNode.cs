using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The ellipse node is a basic shape node representing an ellipse. Note that a circle is an ellipse where
    /// <c>width == height</c>.
    /// </summary>
    [DataContract]
    public class EllipseNode : VectorNode
    {
        public override string type => NodeType.Ellipse;
        
        /// <summary>
        /// Start and end angles of the ellipse measured clockwise from the x axis, plus the inner radius for donuts.
        /// </summary>
        [DataMember(Name = "arcData")]
        public ArcData arcData { get; set; }
    }
    
    /// <summary>
    /// Information about the arc properties of an ellipse. 0° is the x axis and increasing angles rotate clockwise.
    /// </summary>
    [DataContract]
    public class ArcData
    {
        /// <summary>
        /// Start of the sweep in radians.
        /// </summary>
        [DataMember(Name = "startingAngle")]
        public float startingAngle { get; set; }
        
        /// <summary>
        /// End of the sweep in radians.
        /// </summary>
        [DataMember(Name = "endingAngle")]
        public float endingAngle { get; set; }
        
        /// <summary>
        /// Inner radius value between 0 and 1.
        /// </summary>
        [DataMember(Name = "innerRadius")]
        public float innerRadius { get; set; }
    }
}