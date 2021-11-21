using System;

namespace Cdm.Figma
{
    [Serializable]
    public class ComponentSetNode : FrameNode
    {
        public override NodeType type => NodeType.ComponentSet;
    }
}