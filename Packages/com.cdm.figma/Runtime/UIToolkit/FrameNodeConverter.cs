using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    /// <summary>
    /// todo list:
    /// 1-add grid layout
    /// </summary>
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            var frameNode = (FrameNode) node;
            var nodeElement = GroupNodeConverter.Convert(parentElement, frameNode, args);
            BuildStyle(frameNode, nodeElement.inlineStyle);
            return nodeElement;
        }

        private void BuildStyle(FrameNode node, Style style)
        {
            //TODO: handleGridLayout
        }
    }
}