using System;

namespace Cdm.Figma
{
    [Serializable]
    public class LineNode : VectorNode
    {
        public override NodeType type => NodeType.Line;
    }
}