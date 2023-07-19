using Cdm.Figma.Utils;

namespace Cdm.Figma.UI
{
    public class RectangleNodeConverter : VectorNodeConverter<RectangleNode, FigmaNode>
    {
        protected override FigmaNode Convert(FigmaNode parentObject, RectangleNode node, NodeConvertArgs args)
        {
            var convertArgs = new VectorConvertArgs();
            convertArgs.generateSprite = true;
            convertArgs.sourceSprite = VectorNodeConverter.GenerateSprite(node, null, SpriteGenerateType.Rectangle, args);
            return Convert(parentObject, node, args, convertArgs);
        }
    }
}