using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(TextNodeConverter), menuName = AssetMenuRoot + "Text", order = AssetMenuOrder)]
    public class TextNodeConverter : NodeConverter<TextNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}