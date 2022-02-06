namespace Cdm.Figma.UI
{
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        public static NodeObject Convert(NodeObject parentObject, FrameNode node, NodeConvertArgs args)
        {
            var nodeObject = GroupNodeConverter.Convert(parentObject, node, args);
            return nodeObject;
        }
        
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (FrameNode) node, args);
        }
    }
}