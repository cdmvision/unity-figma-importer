using System;

namespace Cdm.Figma
{
    [Serializable]
    public class StarNode : VectorNode
    {
        public override NodeType type => NodeType.Star;
    }
}