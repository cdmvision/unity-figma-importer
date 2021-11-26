using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class LineNode : VectorNode
    {
        public override string type => NodeType.Line;
    }
}