using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(RectangleNodeConverter), 
        menuName = AssetMenuRoot + "Rectangle", order = AssetMenuOrder)]
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((RectangleNode) node, args);
        }
    }
}