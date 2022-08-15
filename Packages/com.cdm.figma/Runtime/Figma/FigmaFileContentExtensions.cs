using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.Utils;

namespace Cdm.Figma
{
    public static class FigmaFileContentExtensions
    {
        public static string[] GetUsedFonts(this FigmaFileContent fileContent)
        {
            var fonts = new HashSet<string>();
            fileContent.document.Traverse(node =>
            {
                var style = ((TextNode)node).style;
                var fontName = FontHelpers.GetFontDescriptor(style.fontFamily, style.fontWeight, style.italic);
                fonts.Add(fontName);
                return true;
            }, NodeType.Text);

            return fonts.ToArray();
        }
    }
}