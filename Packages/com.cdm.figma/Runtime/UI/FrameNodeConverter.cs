namespace Cdm.Figma.UI
{
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            var frameNode = (FrameNode) node;
            var nodeElement = GroupNodeConverter.Convert(parentObject, frameNode, args);
            // TODO: Build style
            return nodeElement;
        }
    }
}