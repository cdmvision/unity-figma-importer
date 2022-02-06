using TMPro;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class TextNodeConverter : NodeConverter<TextNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            
            var textNode = (TextNode) node;

            var nodeObject = VectorNodeConverter.Convert(parentObject, textNode, args);
            var text = nodeObject.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = textNode.characters;
            text.paragraphSpacing = textNode.style.paragraphSpacing;
            // TODO: textNode.style.paragraphIndent;
            // TODO:  textNode.style.listSpacing;
            // TODO: text.fontWeight = textNode.style.fontWeight;
            
            if (textNode.style.italic)
            {
                text.fontStyle |= FontStyles.Italic;
            }
            
            return nodeObject;
        }
    }
}