using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The polygon node allows you to create a regular convex polygon with three or more sides.
    /// </summary>
    [DataContract]
    public class PolygonNode : VectorNode
    {
        public override NodeType type => NodeType.Polygon;
    }
}