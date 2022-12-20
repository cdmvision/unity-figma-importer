using System.Linq;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        protected override FigmaNode Convert(FigmaNode parentObject, FrameNode frameNode, NodeConvertArgs args)
        {
            var figmaNode = args.importer.CreateFigmaNode<FigmaNode>(frameNode);
            figmaNode.SetTransform(frameNode);

            // Frame node's parent may be a page so check if it is INodeTransform.
            if (frameNode.parent is INodeTransform parent)
            {
                figmaNode.SetLayoutConstraints(parent);
            }

            GenerateStyles(figmaNode, frameNode, args);

            figmaNode.ApplyStyles();

            AddLayoutComponentIfNeeded(figmaNode, frameNode);
            AddContentSizeFitterIfNeeded(figmaNode, frameNode);
            AddGridIfNeeded(figmaNode, frameNode);

            BuildChildren(frameNode, figmaNode, args);
            
            if (figmaNode != null && frameNode.isMask)
            {
                args.importer.LogWarning("Frame node with mask is not supported.", figmaNode);
            }
            return figmaNode;
        }

        private static void GenerateStyles(FigmaNode nodeObject, FrameNode node, NodeConvertArgs args)
        {
            Sprite sprite = null;
            if (node.fills.Any() || node.strokes.Any())
            {
                var options = new SpriteGenerateOptions()
                {
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    sampleCount = 8,
                    textureSize = 1024
                };

                if (!args.importer.generatedAssets.TryGet(node.id, out sprite))
                {
                    sprite = NodeSpriteGenerator.GenerateSprite(args.file, node, SpriteGenerateType.Rectangle, options);
                    
                    if (sprite != null)
                    {
                        args.importer.generatedAssets.Add(node.id, sprite);
                        args.importer.generatedAssets.Add(node.id, sprite.texture);
                    }
                }
            }

            {
                var style = new ImageStyle();
                style.componentEnabled.enabled = true;
                style.componentEnabled.value = sprite != null;

                style.sprite.enabled = true;
                style.sprite.value = sprite;

                style.imageType.enabled = true;
                style.imageType.value = Image.Type.Sliced;
                nodeObject.styles.Add(style);
            }
            
            {
                var style = new CanvasGroupStyle();
                style.enabled = true;
                style.alpha.enabled = true;
                style.alpha.value = node.opacity;
                nodeObject.styles.Add(style);
            }
            
            // Add mask if enabled.
            if (node.clipsContent)
            {
                if (node.cornerRadius.HasValue || node.rectangleCornerRadii != null)
                {
                    var style = new MaskStyle();
                    style.enabled = true;
                    nodeObject.styles.Add(style);
                }
                else
                {
                    var style = new RectMaskStyle();
                    style.enabled = true;
                    nodeObject.styles.Add(style);
                }
                
            }

            args.importer.ConvertEffects(nodeObject, node.effects);
        }

        private static void BuildChildren(FrameNode currentNode, FigmaNode nodeObject, NodeConvertArgs args)
        {
            var isAutoLayout = nodeObject.GetComponent<LayoutGroup>() != null;
            
            var children = currentNode.children;
            if (children != null)
            {
                for (var child = 0; child < children.Length; child++)
                {
                    if (children[child].IsIgnored())
                        continue;
                    
                    if (args.importer.TryConvertNode(nodeObject, children[child], args, out var childObject))
                    {
                        if (currentNode.layoutMode != LayoutMode.None)
                        {
                            childObject.gameObject.AddComponent<LayoutElement>();
                            HandleFillContainer(currentNode.layoutMode, nodeObject, childObject);
                        }

                        childObject.rectTransform.SetParent(nodeObject.rectTransform, false);
                        childObject.AdjustPosition(currentNode.size);
                        
                        // Do not add transform style if frame has any auto layout component.
                        if (!isAutoLayout)
                        {
                            // Transform importing is disabled due to a bug right now.
                            // Add transform style after all changes made on rect transform.
                            //childObject.styles.Add(TransformStyle.GetTransformStyle(childObject.rectTransform));    
                        }
                    }
                }
            }
        }

        private static void HandleFillContainer(LayoutMode layoutMode, FigmaNode nodeObject, FigmaNode childElement)
        {
            INodeLayout childLayout = (INodeLayout)childElement.node;
            INodeTransform childTransform = (INodeTransform)childElement.node;

            if (childLayout.layoutAlign == LayoutAlign.Stretch)
            {
                if (layoutMode == LayoutMode.Horizontal)
                {
                    nodeObject.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
                    childElement.gameObject.GetComponent<LayoutElement>().flexibleHeight = 1;
                }
                else if (layoutMode == LayoutMode.Vertical)
                {
                    nodeObject.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
                    childElement.gameObject.GetComponent<LayoutElement>().flexibleWidth = 1;
                }
            }
            else
            {
                if (layoutMode == LayoutMode.Horizontal)
                {
                    //nodeObject.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
                    childElement.gameObject.GetComponent<LayoutElement>().minHeight = childTransform.size.y;
                }
                else
                {
                    //nodeObject.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
                    childElement.gameObject.GetComponent<LayoutElement>().minWidth = childTransform.size.x;
                }
            }

            if (childLayout.layoutGrow.HasValue && childLayout.layoutGrow != 0)
            {
                if (layoutMode == LayoutMode.Horizontal)
                {
                    nodeObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
                    childElement.gameObject.GetComponent<LayoutElement>().flexibleWidth = 1;
                    childElement.gameObject.GetComponent<LayoutElement>().minWidth = 1;
                }
                else if (layoutMode == LayoutMode.Vertical)
                {
                    nodeObject.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                    childElement.gameObject.GetComponent<LayoutElement>().flexibleHeight = 1;
                    childElement.gameObject.GetComponent<LayoutElement>().minHeight = 1;
                }
            }
            else
            {
                if (layoutMode == LayoutMode.Horizontal)
                {
                    //nodeObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
                    childElement.gameObject.GetComponent<LayoutElement>().minWidth = childTransform.size.x;
                }
                else
                {
                    //nodeObject.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                    childElement.gameObject.GetComponent<LayoutElement>().minHeight = childTransform.size.y;
                }
            }
        }

        private static void AddContentSizeFitterIfNeeded(FigmaNode nodeObject, FrameNode groupNode)
        {
            if (groupNode.layoutMode == LayoutMode.None)
                return;

            if (groupNode.primaryAxisSizingMode == AxisSizingMode.Auto ||
                groupNode.counterAxisSizingMode == AxisSizingMode.Auto)
            {
                nodeObject.gameObject.AddComponent<ContentSizeFitter>();
            }

            if (groupNode.primaryAxisSizingMode == AxisSizingMode.Auto)
            {
                if (groupNode.layoutMode == LayoutMode.Horizontal)
                {
                    nodeObject.gameObject.GetComponent<ContentSizeFitter>().horizontalFit =
                        ContentSizeFitter.FitMode.PreferredSize;
                    if (nodeObject.gameObject.GetComponent<HorizontalLayoutGroup>())
                    {
                        nodeObject.gameObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
                    }
                }
                else
                {
                    nodeObject.gameObject.GetComponent<ContentSizeFitter>().verticalFit =
                        ContentSizeFitter.FitMode.PreferredSize;
                    if (nodeObject.gameObject.GetComponent<HorizontalLayoutGroup>())
                    {
                        nodeObject.gameObject.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
                    }
                }
            }

            if (groupNode.counterAxisSizingMode == AxisSizingMode.Auto)
            {
                if (groupNode.layoutMode == LayoutMode.Horizontal)
                {
                    nodeObject.gameObject.GetComponent<ContentSizeFitter>().verticalFit =
                        ContentSizeFitter.FitMode.PreferredSize;
                    if (nodeObject.gameObject.GetComponent<HorizontalLayoutGroup>())
                    {
                        nodeObject.gameObject.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
                    }
                }
                else
                {
                    nodeObject.gameObject.GetComponent<ContentSizeFitter>().horizontalFit =
                        ContentSizeFitter.FitMode.PreferredSize;
                    if (nodeObject.gameObject.GetComponent<HorizontalLayoutGroup>())
                    {
                        nodeObject.gameObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
                    }
                }
            }
        }

        private static void AddLayoutComponentIfNeeded(FigmaNode nodeObject, FrameNode groupNode)
        {
            var layoutMode = groupNode.layoutMode;
            if (layoutMode == LayoutMode.None)
                return;

            HorizontalOrVerticalLayoutGroup layoutGroup = null;

            if (layoutMode == LayoutMode.Horizontal)
            {
                layoutGroup = nodeObject.gameObject.AddComponent<HorizontalLayoutGroup>();

                if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleRight;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperCenter;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerCenter;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                    }
                }
            }
            else
            {
                layoutGroup = nodeObject.gameObject.AddComponent<VerticalLayoutGroup>();

                if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperCenter;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerCenter;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperCenter;
                    }
                }
            }

            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childScaleWidth = false;
            layoutGroup.childScaleHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;

            if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
            {
                if (layoutMode == LayoutMode.Vertical)
                {
                    layoutGroup.childForceExpandHeight = true;
                }
                else if (layoutMode == LayoutMode.Horizontal)
                {
                    layoutGroup.childForceExpandWidth = true;
                }
            }

            // Set padding.
            nodeObject.GetComponent<LayoutGroup>().padding = new RectOffset(
                (int)groupNode.paddingLeft,
                (int)groupNode.paddingRight,
                (int)groupNode.paddingTop,
                (int)groupNode.paddingBottom);

            // Set spacing.
            layoutGroup.spacing = groupNode.itemSpacing;
        }

        private static void AddGridIfNeeded(FigmaNode nodeObject, FrameNode frameNode)
        {
            var visibleGridLayoutCount = frameNode.layoutGrids.Count(x => x.visible);
            if (visibleGridLayoutCount == 1 || visibleGridLayoutCount == 2)
            {
                int colGrids = 0;
                int rowGrids = 0;
                int grids = 0;
                foreach (var grid in frameNode.layoutGrids)
                {
                    if (grid.pattern == Pattern.Columns)
                    {
                        colGrids++;
                        if (colGrids > 2)
                        {
                            Debug.Log("Frame cannot have more than 2 Figma column grid, skipping.");
                            return;
                        }
                    }
                    else if(grid.pattern == Pattern.Rows)
                    {
                        rowGrids++;
                        if (rowGrids > 2)
                        {
                            Debug.Log("Frame cannot have more than 2 Figma row grid, skipping.");
                            return;
                        }
                    }
                    else if (grid.pattern == Pattern.Grid)
                    {
                        grids++;
                        if (grids > 1)
                        {
                            Debug.Log("Frame cannot have more than 1 Figma grid, skipping.");
                            return;
                        }
                    }
                }
                
                var gridView = nodeObject.gameObject.AddComponent<GridLayoutGroup>();
                var alignmentCol = Alignment.Min;
                var alignmentRow = Alignment.Min;
                foreach (var grid in frameNode.layoutGrids)
                {
                    if (grid.pattern == Pattern.Columns)
                    {
                        alignmentCol = grid.alignment;
                        gridView.spacing = new Vector2(grid.gutterSize, gridView.spacing.y);
                        gridView.padding.left = (int)grid.offset;
                        gridView.padding.right = (int)grid.offset;
                        gridView.cellSize = new Vector2(grid.sectionSize, gridView.cellSize.y);
                    }
                    else if (grid.pattern == Pattern.Rows)
                    {
                        alignmentRow = grid.alignment;
                        gridView.spacing = new Vector2(gridView.spacing.x, grid.gutterSize);
                        gridView.padding.top = (int)grid.offset;
                        gridView.padding.bottom = (int)grid.offset;
                        gridView.cellSize = new Vector2(gridView.cellSize.x, grid.sectionSize);
                    }
                    else if (grid.pattern == Pattern.Grid)
                    {
                        gridView.cellSize = new Vector2(grid.sectionSize, grid.sectionSize);
                    }
                }

                if (alignmentCol == Alignment.Min)
                {
                    if (alignmentRow == Alignment.Min)
                    {
                        gridView.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (alignmentRow == Alignment.Center)
                    {
                        gridView.childAlignment = TextAnchor.MiddleLeft;
                    }
                    else if (alignmentRow == Alignment.Max)
                    {
                        gridView.childAlignment = TextAnchor.LowerLeft;
                    }
                }
                else if (alignmentCol == Alignment.Center)
                {
                    if (alignmentRow == Alignment.Min)
                    {
                        gridView.childAlignment = TextAnchor.UpperCenter;
                    }
                    else if (alignmentRow == Alignment.Center)
                    {
                        gridView.childAlignment = TextAnchor.MiddleCenter;
                    }
                    else if (alignmentRow == Alignment.Max)
                    {
                        gridView.childAlignment = TextAnchor.LowerCenter;
                    }
                }
                else if (alignmentCol == Alignment.Max)
                {
                    if (alignmentRow == Alignment.Min)
                    {
                        gridView.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (alignmentRow == Alignment.Center)
                    {
                        gridView.childAlignment = TextAnchor.MiddleRight;
                    }
                    else if (alignmentRow == Alignment.Max)
                    {
                        gridView.childAlignment = TextAnchor.LowerRight;
                    }
                }
            }
        }
    }
}