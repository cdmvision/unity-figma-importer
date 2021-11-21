using System;

namespace Cdm.Figma
{
    [Serializable]
    public class RegularPolygonNode : VectorNode
    {
        public override NodeType type => NodeType.RegularPolygon;
    }
}