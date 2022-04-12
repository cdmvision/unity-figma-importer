using Cdm.Figma.Utils;

namespace Cdm.Figma.UI
{
    public class RectangleNodeConverter : VectorNodeConverter<RectangleNode>
    {
        protected override NodeObject Convert(NodeObject parentObject, RectangleNode node, NodeConvertArgs args)
        {
            var convertArgs = new VectorConvertArgs();
            convertArgs.generateSprite = true;
            convertArgs.sourceSprite = VectorImageUtils.CreateSpriteFromRect(node, convertArgs.spriteOptions);
            return Convert(parentObject, node, args, convertArgs);
        }
    }
}