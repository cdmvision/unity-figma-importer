using System;

namespace Cdm.Figma
{
    [Serializable]
    public class ComponentNode : FrameNode
    {
        public override NodeType type => NodeType.Component;
    }
}