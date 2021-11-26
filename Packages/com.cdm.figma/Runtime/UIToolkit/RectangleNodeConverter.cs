using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(RectangleNodeConverter), menuName = AssetMenuRoot + "Rectangle", order = 20)]
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        private const string xmlType = "ui:VisualElement";
        public override XElement Convert(FigmaImporter importer, FigmaFile file, Node node)
        {
            var rectangleNode = (RectangleNode) node;
            var rectangleElement = new XElement(xmlType, new XAttribute("name", rectangleNode.name));
            Debug.Log($"Rectangle XML element: {rectangleElement}");            
            return rectangleElement;
        }
    }
}