using UnityEngine;

namespace Cdm.Figma.UI
{
    public class StarNodeConverter : NodeConverter<StarNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((StarNode) node, args);
        }
    }
}