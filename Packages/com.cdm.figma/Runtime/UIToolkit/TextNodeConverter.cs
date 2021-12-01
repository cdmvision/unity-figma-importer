using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(TextNodeConverter), menuName = AssetMenuRoot + "Text", order = AssetMenuOrder)]
    public class TextNodeConverter : NodeConverter<TextNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            var textNode = (TextNode) node;
            var label = XmlFactory.NewElement<Label>(node, args);
            label.Value = textNode.characters;
            return label;
        }
    }
}