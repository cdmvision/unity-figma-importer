using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class RegularPolygonNode : VectorNode
    {
        public override string type => NodeType.RegularPolygon;
    }
}