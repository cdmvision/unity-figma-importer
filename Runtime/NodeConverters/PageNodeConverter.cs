using UnityEngine;

namespace Cdm.Figma.UI
{
    public class PageNodeConverter : NodeConverter<PageNode>
    {
        protected override FigmaNode Convert(FigmaNode parentObject, PageNode pageNode, NodeConvertArgs args)
        {
            var figmaPage = args.importer.CreateFigmaNode<FigmaPage>(pageNode);
            figmaPage.rectTransform.anchorMin = new Vector2(0, 0);
            figmaPage.rectTransform.anchorMax = new Vector2(1, 1);
            figmaPage.rectTransform.offsetMin = new Vector2(0, 0);
            figmaPage.rectTransform.offsetMax = new Vector2(0, 0);
            
            foreach (var node in pageNode.children)
            {
                if (node.IsIgnored())
                    continue;

                if (node is FrameNode)
                {
                    if (args.importer.TryConvertNode(figmaPage, node, args, out var frameNode))
                    {
                        frameNode.rectTransform.anchorMin = new Vector2(0, 0);
                        frameNode.rectTransform.anchorMax = new Vector2(1, 1);
                        frameNode.rectTransform.offsetMin = new Vector2(0, 0);
                        frameNode.rectTransform.offsetMax = new Vector2(0, 0);
                        frameNode.transform.SetParent(figmaPage.rectTransform, false);
                    }
                }
            }
            
            return figmaPage;
        }
    }
}