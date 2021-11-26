using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(RectangleNodeConverter), menuName = AssetMenuRoot + "Rectangle", order = 20)]
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            var rectangleNode = (RectangleNode) node;
            var rectangleElement = 
                new XElement(args.namespaces.engine + nameof(VisualElement), 
                    new XAttribute("name", rectangleNode.name));
            Debug.Log($"Rectangle XML element: {rectangleElement}");            
            return rectangleElement;
        }
    }
}