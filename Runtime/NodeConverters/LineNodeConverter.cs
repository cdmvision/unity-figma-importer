namespace Cdm.Figma.UI
{
    public class LineNodeConverter : VectorNodeConverter<LineNode, FigmaNode>
    {
        protected override FigmaNode Convert(FigmaNode parentObject, LineNode vectorNode, NodeConvertArgs args)
        {
            var figmaNode = base.Convert(parentObject, vectorNode, args);
            if (figmaNode != null)
            {
                args.importer.LogWarning(
                    "Use 'Outline stroke' on the line node in the Figma editor before importing the document.",
                    figmaNode);
            }

            return figmaNode;
        }
    }
}