using System;

namespace Cdm.Figma
{
    [Serializable]
    public class UnsupportedNode : Node
    {
        public override NodeType type => NodeType.Unsupported;
    }
}