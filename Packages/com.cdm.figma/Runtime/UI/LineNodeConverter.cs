using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(LineNodeConverter), menuName = AssetMenuRoot + "Line", order = AssetMenuOrder)]
    public class LineNodeConverter : NodeConverter<LineNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((LineNode) node, args);
        }
    }
}