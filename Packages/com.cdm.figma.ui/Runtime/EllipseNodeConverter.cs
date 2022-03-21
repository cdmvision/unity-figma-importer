﻿namespace Cdm.Figma.UI
{
    public class EllipseNodeConverter : NodeConverter<EllipseNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert(parentObject, (EllipseNode) node, args);
        }
    }
}