namespace Cdm.Figma.UI
{
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        public static NodeObject Convert(FrameNode node, NodeConvertArgs args)
        {
            var nodeObject = GroupNodeConverter.Convert(node, args);
            // TODO: frameNode.layoutGrids;
            return nodeObject;
        }
        
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return Convert((FrameNode) node, args);
        }
    }
}