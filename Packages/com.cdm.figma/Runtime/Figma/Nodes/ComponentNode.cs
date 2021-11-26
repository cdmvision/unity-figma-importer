using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentNode : FrameNode
    {
        public override string type => NodeType.Component;
    }
}