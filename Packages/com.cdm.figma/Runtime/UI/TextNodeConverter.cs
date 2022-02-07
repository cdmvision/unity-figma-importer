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
            text.characterSpacing = textNode.style.letterSpacing;
            text.fontSize = textNode.style.fontSize;
            
            if (textNode.fills != null && textNode.fills.Length > 0)
            {
                if (textNode.fills[0] is SolidPaint solidPaint)
                {
                    text.color = solidPaint.color;
                }
            }
            
            var fontName = 
                FontSource.GetFontName(textNode.style.fontFamily, textNode.style.fontWeight, textNode.style.italic);
            
            if (args.file.TryGetFont(fontName, out var font))
            {
                text.font = font;
            }
            else
            {
                Debug.LogWarning($"{fontName} could not be found.");
            }
            
            if (textNode.style.italic)
            {
                text.fontStyle |= FontStyles.Italic;
            }
            return nodeObject;
        }
    }
}