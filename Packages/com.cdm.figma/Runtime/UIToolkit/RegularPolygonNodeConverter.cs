using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(RegularPolygonNodeConverter), 
        menuName = AssetMenuRoot + "Regular Polygon", order = AssetMenuOrder)]
    public class RegularPolygonNodeConverter : NodeConverter<PolygonNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((PolygonNode) node, args);
        }
    }
}