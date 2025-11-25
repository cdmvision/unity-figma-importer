using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.Utils;

namespace Cdm.Figma
{
    public static class FigmaFileExtensions
    {
        public static string[] GetUsedFonts(this FigmaFile file)
        {
            var fonts = new HashSet<string>();
            file.document.TraverseDfs(node =>
            {
                var style = ((TextNode)node).style;
                var fontName = FontHelpers.GetFontDescriptor(style.fontFamily, style.fontWeight, style.italic);
                fonts.Add(fontName);
                return true;
            }, NodeType.Text);

            return fonts.ToArray();
        }

        public static bool IsBranch(this FigmaFile file)
        {
            return !string.IsNullOrEmpty(file.mainFileKey);
        }
    }
}