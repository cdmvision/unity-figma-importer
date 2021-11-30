using Unity.VectorGraphics.Editor;
using UnityEditor;

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

        protected override void ImportGraphic(string assetPath)
        {
            var svgImporter = (SVGImporter) AssetImporter.GetAtPath(assetPath);
            svgImporter.PreserveSVGImageAspect = true;
            svgImporter.SvgType = SVGType.UIToolkit;

            EditorUtility.SetDirty(svgImporter);
            svgImporter.SaveAndReimport();
        }
    }
}