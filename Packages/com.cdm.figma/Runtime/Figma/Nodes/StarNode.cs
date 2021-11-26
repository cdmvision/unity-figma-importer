using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class StarNode : VectorNode
    {
        public override string type => NodeType.Star;
    }
}