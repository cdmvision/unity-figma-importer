using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;

namespace Cdm.Figma.UI
{
    [CustomEditor(typeof(FigmaTaskFile))]
    public class FigmaTaskFileEditor : Cdm.Figma.FigmaTaskFileEditor
    {
        protected override string GetImplementationName() => nameof(UI);

        protected override Task OnFigmaFileSaving(
            Figma.FigmaTaskFile taskFile, Figma.FigmaFile newFile, Figma.FigmaFile oldFile)
        {
            if (oldFile != null)
            {
                var oldFigmaFile = (FigmaFile) oldFile;
                var newFigmaFile = (FigmaFile) newFile;

                newFigmaFile.fallbackFont = oldFigmaFile.fallbackFont;

                foreach (var fontSource in newFigmaFile.fonts)
                {
                    if (fontSource.font == null)
                    {
                        var oldFont = oldFigmaFile.fonts.FirstOrDefault(x =>
                            x.fontName.Equals(fontSource.fontName, StringComparison.OrdinalIgnoreCase));
                        if (oldFont != null && oldFont.font != null)
                        {
                            fontSource.font = oldFont.font;
                        }
                    }
                }
            }
            
            return Task.CompletedTask;
        }
    }
}
