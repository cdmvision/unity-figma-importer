using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class NodeData
    {
        public XElement element { get; }
        public Style style { get; }

        public NodeData(XName name, params object[] content)
        {
            element = new XElement(name, content);
            style = new Style();
            
            // Figma transform origin starts from top left corner.
            style.transformOrigin = new StyleTransformOrigin(
                new TransformOrigin(new Length(0, LengthUnit.Percent), new Length(0, LengthUnit.Percent), 0f));
        }

        public void UpdateStyle()
        {
            element.SetAttributeValue("style", style.ToString());
        }
    }
}