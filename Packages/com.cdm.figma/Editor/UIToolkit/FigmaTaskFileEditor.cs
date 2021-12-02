using Unity.VectorGraphics.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CustomEditor(typeof(FigmaTaskFile))]
    public class FigmaTaskFileEditor : Cdm.Figma.FigmaTaskFileEditor
    {
        protected override string GetImplementationName() => nameof(UIToolkit);

        protected override string[] GetNodeGraphicTypes()
        {
            return new[] {NodeType.Vector, NodeType.Ellipse, NodeType.Line, NodeType.Polygon, NodeType.Star};
        }

        protected override void ImportGraphic(Figma.FigmaFile file, string graphicName, string graphicAsset)
        {
            var figmaFile = (FigmaFile) file;
            
            var svgImporter = (SVGImporter) AssetImporter.GetAtPath(graphicAsset);
            svgImporter.PreserveSVGImageAspect = true;
            svgImporter.SvgType = SVGType.UIToolkit;

            EditorUtility.SetDirty(svgImporter);
            svgImporter.SaveAndReimport();
            
            var vectorImage = AssetDatabase.LoadAssetAtPath<VectorImage>(graphicAsset);
            figmaFile.graphics.Add(new GraphicSource()
            {
                id = graphicName,
                graphic = vectorImage
            });
        }

        protected override void ImportFont(Figma.FigmaFile file, FontDescriptor fontDescriptor)
        {
            var figmaFile = (FigmaFile) file;
            
            figmaFile.fonts.Add(new FontSource()
            {
                family = fontDescriptor.family,
                weight = fontDescriptor.weight,
                italic = fontDescriptor.italic,
                font = null
            });
        }
    }
}