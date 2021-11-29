using Unity.VectorGraphics.Editor;
using UnityEditor;

namespace Cdm.Figma.UI
{
    [CustomEditor(typeof(FigmaTaskFile))]
    public class FigmaTaskFileEditor : Cdm.Figma.FigmaTaskFileEditor
    {
        protected override string GetImplementationName() => nameof(UI);

        protected override string[] GetNodeGraphicTypes()
        {
            return new[] {NodeType.Vector, NodeType.Line, NodeType.Rectangle, NodeType.RegularPolygon, NodeType.Star};
        }

        protected override void ImportGraphic(string assetPath)
        {
            var svgImporter = (SVGImporter) AssetImporter.GetAtPath(assetPath);
            svgImporter.PreserveSVGImageAspect = true;
            svgImporter.SvgType = SVGType.VectorSprite;

            EditorUtility.SetDirty(svgImporter);
            svgImporter.SaveAndReimport();
        }
    }
}
