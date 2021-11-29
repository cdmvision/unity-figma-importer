using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(RegularPolygonNodeConverter), 
        menuName = AssetMenuRoot + "Regular Polygon", order = AssetMenuOrder)]
    public class RegularPolygonNodeConverter : NodeConverter<RegularPolygonNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((RegularPolygonNode) node, args);
        }
    }
}