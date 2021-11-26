using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentSetNode : FrameNode
    {
        public override string type => NodeType.ComponentSet;
    }
}