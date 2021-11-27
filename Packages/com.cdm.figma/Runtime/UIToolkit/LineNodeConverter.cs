using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(LineNodeConverter), menuName = AssetMenuRoot + "Line", order = 20)]
    public class LineNodeConverter : NodeConverter<LineNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((LineNode) node, args);
        }
    }
}