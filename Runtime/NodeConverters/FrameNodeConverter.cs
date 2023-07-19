using System.Linq;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Utils;
using Cdm.Figma.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        protected override FigmaNode Convert(FigmaNode parentObject, FrameNode frameNode, NodeConvertArgs args)
        {
            var frameNodeObject = args.importer.CreateFigmaNode<FigmaNode>(frameNode);
            frameNodeObject.SetTransform(frameNode);

            // Frame node's parent may be a page so check if it is INodeTransform.
            if (frameNode.parent is INodeTransform parent)
            {
                frameNodeObject.SetLayoutConstraints(parent);
            }

            GenerateStyles(frameNodeObject, frameNode, args);

            frameNodeObject.ApplyStyles();

            AddLayoutComponentIfNeeded(frameNodeObject, frameNode);
            AddContentSizeFitterIfNeeded(frameNodeObject, frameNode);
            AddGridIfNeeded(frameNodeObject, frameNode);

            BuildChildren(frameNode, frameNodeObject, args);
            
            if (frameNodeObject != null && frameNode.isMask)
            {
                args.importer.LogWarning("Frame node with mask is not supported.", frameNodeObject);
            }
            return frameNodeObject;
        }

        private static void GenerateStyles(FigmaNode nodeObject, FrameNode node, NodeConvertArgs args)
        {
            var sprite = VectorNodeConverter.GenerateSprite(node, nodeObject, SpriteGenerateType.Rectangle, args);

            // Add image style.
            {
                var style = new ImageStyle();
                style.componentEnabled.enabled = true;
                style.componentEnabled.value = sprite != null;

                style.sprite.enabled = true;
                style.sprite.value = sprite;

                style.imageType.enabled = true;
                style.imageType.value = sprite.GetImageType();
                nodeObject.styles.Add(style);
            }
            
            // Add canvas group style.
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

        private static void BuildChildren(FrameNode frameNode, FigmaNode frameNodeObject, NodeConvertArgs args)
        {
            var isAutoLayout = frameNodeObject.GetComponent<LayoutGroup>() != null;
            
            var children = frameNode.children;
            if (children != null)
            {
                for (var i = 0; i < children.Length; i++)
                {
                    if (children[i].IsIgnored())
                        continue;

                    Node overrideNode = null;
                    
                    var overrideNodeChildren = args.overrideNode?.GetChildren();
                    if (overrideNodeChildren != null && overrideNodeChildren.Length == children.Length)
                    {
                        overrideNode = overrideNodeChildren[i];
                    }

                    using (args.OverrideNode(overrideNode))
                    {
                        if (args.importer.TryConvertNode(frameNodeObject, children[i], args, out var childObject))
                        {
                            if (frameNode.layoutMode != LayoutMode.None)
                            {
                                HandleFillContainer(frameNode.layoutMode, frameNodeObject, childObject);
                            }

                            childObject.rectTransform.SetParent(frameNodeObject.rectTransform, false);
                            childObject.AdjustPosition(frameNode.size);
                        
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
        }

        private static void HandleFillContainer(LayoutMode layoutMode, FigmaNode nodeObject, FigmaNode child)
        {
            var parent = (FrameNode) nodeObject.node;
            if (parent.layoutMode == LayoutMode.None)
                return;
            
            var layoutGroup = nodeObject.GetComponent<HorizontalOrVerticalLayoutGroup>();
            var layoutElement = child.gameObject.GetOrAddComponent<LayoutElement>();
            
            var childNode = (SceneNode)child.node;
            var childLayout = (INodeLayout)child.node;
            var childTransform = (INodeTransform)child.node;
            
            if (childNode.layoutPositioning == LayoutPositioning.Absolute)
            {
                layoutElement.ignoreLayout = true;
            }
            
            if (childLayout.layoutAlign == LayoutAlign.Stretch)
            {
                if (layoutMode == LayoutMode.Horizontal)
                {
                    layoutGroup.childControlHeight = true;
                    layoutElement.flexibleHeight = 1;
                }
                else if (layoutMode == LayoutMode.Vertical)
                {
                    layoutGroup.childControlWidth = true;
                    layoutElement.flexibleWidth = 1;
                }
            }
            else
            {
                if (layoutMode == LayoutMode.Horizontal)
                {
                    //layoutGroup.childControlHeight = true;
                    layoutElement.minHeight = childTransform.size.y;
                }
                else
                {
                    //layoutGroup.childControlWidth = true;
                    layoutElement.minWidth = childTransform.size.x;
                }
            }

            if (childLayout.layoutGrow.HasValue && childLayout.layoutGrow != 0)
            {
                if (layoutMode == LayoutMode.Horizontal)
                {
                    layoutGroup.childControlWidth = true;
                    layoutElement.flexibleWidth = 1;
                    layoutElement.minWidth = 1;
                }
                else if (layoutMode == LayoutMode.Vertical)
                {
                    layoutGroup.childControlHeight = true;
                    layoutElement.flexibleHeight = 1;
                    layoutElement.minHeight = 1;
                }
            }
            else
            {
                if (layoutMode == LayoutMode.Horizontal)
                {
                    layoutGroup.childControlWidth = true;
                    layoutElement.minWidth = childTransform.size.x;
                }
                else
                {
                    layoutGroup.childControlHeight = true;
                    layoutElement.minHeight = childTransform.size.y;
                }
            }

            var contentSizeFitter = child.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter)
            {
                if (contentSizeFitter.horizontalFit == ContentSizeFitter.FitMode.PreferredSize)
                {
                    layoutElement.minWidth = 0;
                }

                if (contentSizeFitter.verticalFit == ContentSizeFitter.FitMode.PreferredSize)
                {
                    layoutElement.minHeight = 0;
                }
            }
        }

        private static void AddContentSizeFitterIfNeeded(FigmaNode nodeObject, FrameNode frameNode)
        {
            if (frameNode.layoutMode == LayoutMode.None)
                return;

            if (frameNode.primaryAxisSizingMode != AxisSizingMode.Auto &&
                frameNode.counterAxisSizingMode != AxisSizingMode.Auto)
                return;
            
            var contentSizeFitter = nodeObject.gameObject.GetOrAddComponent<ContentSizeFitter>();
            var horizontalLayoutGroup = nodeObject.gameObject.GetComponent<HorizontalLayoutGroup>();
            
            if (frameNode.primaryAxisSizingMode == AxisSizingMode.Auto)
            {
                if (frameNode.layoutMode == LayoutMode.Horizontal)
                {
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                    if (horizontalLayoutGroup != null)
                    {
                        horizontalLayoutGroup.childControlWidth = true;
                    }
                }
                else
                {
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                   
                    if (horizontalLayoutGroup != null)
                    {
                        horizontalLayoutGroup.childControlHeight = true;
                    }
                }
            }

            if (frameNode.counterAxisSizingMode == AxisSizingMode.Auto)
            {
                if (frameNode.layoutMode == LayoutMode.Horizontal)
                {
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    
                    if (horizontalLayoutGroup != null)
                    {
                        horizontalLayoutGroup.childControlHeight = true;
                    }
                }
                else
                {
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    
                    if (horizontalLayoutGroup != null)
                    {
                        horizontalLayoutGroup.childControlWidth = true;
                    }
                }
            }
        }

        private static void AddLayoutComponentIfNeeded(FigmaNode nodeObject, FrameNode frameNode)
        {
            var layoutMode = frameNode.layoutMode;
            if (layoutMode == LayoutMode.None)
                return;

            HorizontalOrVerticalLayoutGroup layoutGroup = null;

            if (layoutMode == LayoutMode.Horizontal)
            {
                layoutGroup = nodeObject.gameObject.AddComponent<HorizontalLayoutGroup>();

                if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
                {
                    if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerLeft;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                    }
                }
                else if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
                {
                    if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerRight;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleRight;
                    }
                }
                else if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
                {
                    if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperCenter;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerCenter;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    }
                }
                else if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
                {
                    if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerLeft;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                    }
                }
            }
            else
            {
                layoutGroup = nodeObject.gameObject.AddComponent<VerticalLayoutGroup>();

                if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
                {
                    if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperCenter;
                    }
                }
                else if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
                {
                    if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerLeft;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerRight;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerCenter;
                    }
                }
                else if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
                {
                    if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleRight;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    }
                }
                else if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
                {
                    if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
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

            if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
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
            layoutGroup.padding = new RectOffset(
                left: Mathf.RoundToInt(frameNode.paddingLeft),
                right: Mathf.RoundToInt(frameNode.paddingRight),
                top: Mathf.RoundToInt(frameNode.paddingTop),
                bottom: Mathf.RoundToInt(frameNode.paddingBottom));

            // Set spacing.
            layoutGroup.spacing = frameNode.itemSpacing;
        }

        private static void AddGridIfNeeded(FigmaNode nodeObject, FrameNode frameNode)
        {
            var visibleGridLayoutCount = frameNode.layoutGrids.Count(x => x.visible);
            if (visibleGridLayoutCount == 1 || visibleGridLayoutCount == 2)
            {
                var colGrids = 0;
                var rowGrids = 0;
                var grids = 0;
                
                foreach (var grid in frameNode.layoutGrids)
                {
                    if (grid.pattern == Pattern.Columns)
                    {
                        colGrids++;
                        if (colGrids > 2)
                        {
                            Debug.LogWarning("Frame cannot have more than 2 Figma column grid, skipping.");
                            return;
                        }
                    }
                    else if(grid.pattern == Pattern.Rows)
                    {
                        rowGrids++;
                        if (rowGrids > 2)
                        {
                            Debug.LogWarning("Frame cannot have more than 2 Figma row grid, skipping.");
                            return;
                        }
                    }
                    else if (grid.pattern == Pattern.Grid)
                    {
                        grids++;
                        if (grids > 1)
                        {
                            Debug.LogWarning("Frame cannot have more than 1 Figma grid, skipping.");
                            return;
                        }
                    }
                }

                // If there is another layout group added already.
                var layoutGroup = nodeObject.gameObject.GetComponent<LayoutGroup>();
                if (layoutGroup != null && layoutGroup is not GridLayoutGroup)
                    return;
                
                var gridLayoutGroup = nodeObject.gameObject.GetOrAddComponent<GridLayoutGroup>();
                var alignmentCol = Alignment.Min;
                var alignmentRow = Alignment.Min;
                
                foreach (var grid in frameNode.layoutGrids)
                {
                    if (grid.pattern == Pattern.Columns)
                    {
                        alignmentCol = grid.alignment;
                        gridLayoutGroup.spacing = new Vector2(grid.gutterSize, gridLayoutGroup.spacing.y);
                        gridLayoutGroup.padding.left = (int)grid.offset;
                        gridLayoutGroup.padding.right = (int)grid.offset;
                        gridLayoutGroup.cellSize = new Vector2(grid.sectionSize, gridLayoutGroup.cellSize.y);
                    }
                    else if (grid.pattern == Pattern.Rows)
                    {
                        alignmentRow = grid.alignment;
                        gridLayoutGroup.spacing = new Vector2(gridLayoutGroup.spacing.x, grid.gutterSize);
                        gridLayoutGroup.padding.top = (int)grid.offset;
                        gridLayoutGroup.padding.bottom = (int)grid.offset;
                        gridLayoutGroup.cellSize = new Vector2(gridLayoutGroup.cellSize.x, grid.sectionSize);
                    }
                    else if (grid.pattern == Pattern.Grid)
                    {
                        gridLayoutGroup.cellSize = new Vector2(grid.sectionSize, grid.sectionSize);
                    }
                }

                if (alignmentCol == Alignment.Min)
                {
                    if (alignmentRow == Alignment.Min)
                    {
                        gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (alignmentRow == Alignment.Center)
                    {
                        gridLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
                    }
                    else if (alignmentRow == Alignment.Max)
                    {
                        gridLayoutGroup.childAlignment = TextAnchor.LowerLeft;
                    }
                }
                else if (alignmentCol == Alignment.Center)
                {
                    if (alignmentRow == Alignment.Min)
                    {
                        gridLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                    }
                    else if (alignmentRow == Alignment.Center)
                    {
                        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    }
                    else if (alignmentRow == Alignment.Max)
                    {
                        gridLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                    }
                }
                else if (alignmentCol == Alignment.Max)
                {
                    if (alignmentRow == Alignment.Min)
                    {
                        gridLayoutGroup.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (alignmentRow == Alignment.Center)
                    {
                        gridLayoutGroup.childAlignment = TextAnchor.MiddleRight;
                    }
                    else if (alignmentRow == Alignment.Max)
                    {
                        gridLayoutGroup.childAlignment = TextAnchor.LowerRight;
                    }
                }
            }
        }
    }
}