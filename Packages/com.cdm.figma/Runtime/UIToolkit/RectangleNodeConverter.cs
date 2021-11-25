using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(RectangleNodeConverter), menuName = AssetMenuRoot + "Rectangle", order = 20)]
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override XElement Convert(FigmaImporter importer, FigmaFile file, Node node)
        {
            var rectangleNode = (RectangleNode) node;
            // TODO: Implement
            throw new System.NotImplementedException();
        }
    }
}