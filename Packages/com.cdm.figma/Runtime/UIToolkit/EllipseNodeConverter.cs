using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(EllipseNodeConverter), 
        menuName = AssetMenuRoot + "Ellipse", order = AssetMenuOrder)]
    public class EllipseNodeConverter: NodeConverter<EllipseNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((EllipseNode) node, args);
        }
    }
}