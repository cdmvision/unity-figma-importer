using System.Xml;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(RectangleNodeConverter), menuName = AssetMenuRoot + "Rectangle", order = 21)]
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override XmlElement Convert(FigmaImporter importer, FigmaFile file, Node node)
        {
            var rectangleNode = (RectangleNode) node;
            // TODO: Implement
            throw new System.NotImplementedException();
        }
    }
}