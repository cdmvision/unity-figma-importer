namespace Cdm.Figma.UIToolkit
{
    /// <summary>
    /// todo list:
    /// 1-add grid layout
    /// </summary>
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        public static NodeElement Convert(NodeElement parentElement, FrameNode frameNode, NodeConvertArgs args)
        {
            var nodeElement = GroupNodeConverter.Convert(parentElement, frameNode, args);
            BuildStyle(frameNode, nodeElement.inlineStyle);
            return nodeElement;
        }
        
        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            return Convert(parentElement, (FrameNode) node, args);
        }

        private static void BuildStyle(FrameNode node, Style style)
        {
            //TODO: handleGridLayout
        }
    }
}