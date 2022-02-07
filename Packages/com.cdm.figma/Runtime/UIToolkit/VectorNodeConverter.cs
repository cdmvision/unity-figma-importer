using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class VectorNodeConverter : NodeConverter<VectorNode>
    {
        public static NodeElement Convert(NodeElement parentElement, VectorNode node, NodeConvertArgs args)
        {
            var element = NodeElement.New<VisualElement>(node, args);
            BuildStyle(node, args, element.inlineStyle);
            return element;
        }

        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            return Convert(parentElement, (VectorNode) node, args);
        }

        private static void BuildStyle(VectorNode node, NodeConvertArgs args, Style style)
        {
            //this node must be a child of a node so its width, height and size is set in its parent.
            
            SetOpacity(node, style);
            SetRotation(node, style);
            AddBackgroundColor(node, style);
            AddStrokes(node, style);
            SetTransformOrigin(style);
            //style.width = new StyleLength(node.size.x);
            //style.height = new StyleLength(node.size.y);

            // Override width and height if SVG contains these properties.
            if (args.file.TryGetGraphic(node.id, out var graphic))
            {
                style.backgroundImage = new StyleBackground(graphic);
                
#if UNITY_EDITOR
                var path = UnityEditor.AssetDatabase.GetAssetPath(graphic);
                if (!string.IsNullOrEmpty(path))
                {
                    var svg = XDocument.Load(path);
                    if (svg.Root != null)
                    {
                        var widthAttribute = svg.Root.Attribute("width");
                        if (widthAttribute != null)
                        {
                            style.width = new StyleLength(float.Parse(widthAttribute.Value));
                        }

                        var heightAttribute = svg.Root.Attribute("height");
                        if (heightAttribute != null)
                        {
                            style.height = new StyleLength(float.Parse(heightAttribute.Value));
                        }
                    }
                }

#else
                Debug.LogWarning("SVG file cannot be read at runtime.");
#endif
            }
        }
        
        private static void SetOpacity(Node node, Style style)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            var opacity = rectangleNode.opacity;
            style.opacity = new StyleFloat(opacity);
        }

        private static void SetRotation(Node node, Style style)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            var relativeTransform = rectangleNode.relativeTransform;
            var rotation = relativeTransform.GetRotationAngle();
            if (rotation != 0.0f)
            {
                style.rotate = new StyleRotate(new Rotate(rotation));
            }
        }

        private static void SetTransformOrigin(Style style)
        {
            // Figma transform pivot is located on the top left.
            style.transformOrigin =
                new StyleTransformOrigin(new TransformOrigin(Length.Percent(0f), Length.Percent(0f), 0.0f));
        }

        private static void AddStrokes(Node node, Style style)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            var strokes = rectangleNode.strokes;
            if (strokes.Length > 0)
            {
                var strokeWeight = rectangleNode.strokeWeight;
                if (strokeWeight.HasValue)
                {
                    style.borderTopWidth = new StyleFloat((float) strokeWeight);
                    style.borderLeftWidth = new StyleFloat((float) strokeWeight);
                    style.borderBottomWidth = new StyleFloat((float) strokeWeight);
                    style.borderRightWidth = new StyleFloat((float) strokeWeight);
                }

                //only getting the base color
                var solidColor = (SolidPaint) strokes[0];
                var strokeColorBlended = solidColor.color;
                style.borderTopColor = new StyleColor(strokeColorBlended);
                style.borderLeftColor = new StyleColor(strokeColorBlended);
                style.borderBottomColor = new StyleColor(strokeColorBlended);
                style.borderRightColor = new StyleColor(strokeColorBlended);
            }
        }

        private static void AddBackgroundColor(Node node, Style style)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            var fills = rectangleNode.fills;
            if (fills.Length > 0)
            {
                //only getting the base color
                var solidColor = (SolidPaint) fills[0];
                var fillColorBlended = solidColor.color;
                style.backgroundColor = new StyleColor(fillColorBlended);
            }
        }
    }
}