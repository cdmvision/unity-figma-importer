using Cdm.Figma.UI.Styles;
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
            text.color = UnityEngine.Color.white;
            text.text = textNode.characters;
            
            var textStyle = nodeObject.gameObject.AddComponent<TextStyle>();
            textStyle.normal.enabled = true;
            textStyle.normal.characterSpacing.enabled = true;
            textStyle.normal.characterSpacing.value = textNode.style.letterSpacing;
            textStyle.normal.color.enabled = true;
            textStyle.normal.color.value = ((SolidPaint)textNode.fills[0]).color;   // TODO: unsafe
            textStyle.normal.fontSize.enabled = true;
            textStyle.normal.fontSize.value = textNode.style.fontSize;
            //textStyle.normal.fontWeight.enabled = true;
            //textStyle.normal.fontWeight.value = textNode.style.fontWeight;
            
            var fontName = 
                FontSource.GetFontName(textNode.style.fontFamily, textNode.style.fontWeight, textNode.style.italic);
            
            if (args.file.TryGetFont(fontName, out var font))
            {
                textStyle.normal.font.enabled = true;
                textStyle.normal.font.value = font;
            }
            else
            {
                Debug.LogWarning($"{fontName} could not be found.");
            }
            
            if (textNode.style.italic)
            {
                textStyle.normal.fontStyle.enabled = true;
                textStyle.normal.fontStyle.value |= FontStyles.Italic;
            }
            return nodeObject;
        }
    }
}