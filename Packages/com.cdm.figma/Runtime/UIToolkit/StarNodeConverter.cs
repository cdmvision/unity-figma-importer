using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(StarNodeConverter), menuName = AssetMenuRoot + "Star", order = AssetMenuOrder)]
    public class StarNodeConverter : NodeConverter<StarNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((StarNode) node, args);
        }
    }
}