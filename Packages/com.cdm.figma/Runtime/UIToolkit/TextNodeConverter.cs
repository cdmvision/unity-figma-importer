using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(TextNodeConverter), menuName = AssetMenuRoot + "Text", order = 20)]
    public class TextNodeConverter : NodeConverter<TextNode>
    {
        public override XElement Convert(FigmaImporter importer, FigmaFile file, Node node)
        {
            throw new System.NotImplementedException();
        }
    }
}