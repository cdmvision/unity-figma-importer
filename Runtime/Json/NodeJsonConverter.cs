using System;

namespace Cdm.Figma.Json
{
    public class NodeJsonConverter : SubTypeJsonConverter<Node, NodeType>
    {
        protected override string GetTypeToken()
        {
            return nameof(Node.type);
        }

        protected override bool TryGetActualType(NodeType typeToken, out Type type)
        {
            switch (typeToken)
            {
                case NodeType.Boolean:
                    type = typeof(BooleanNode);
                    return true;

                case NodeType.Page:
                    type = typeof(PageNode);
                    return true;

                case NodeType.Component:
                    type = typeof(ComponentNode);
                    return true;

                case NodeType.ComponentSet:
                    type = typeof(ComponentSetNode);
                    return true;

                case NodeType.Document:
                    type = typeof(DocumentNode);
                    return true;

                case NodeType.Ellipse:
                    type = typeof(EllipseNode);
                    return true;

                case NodeType.Frame:
                    type = typeof(FrameNode);
                    return true;

                case NodeType.Group:
                    type = typeof(GroupNode);
                    return true;

                case NodeType.Instance:
                    type = typeof(InstanceNode);
                    return true;

                case NodeType.Rectangle:
                    type = typeof(RectangleNode);
                    return true;

                case NodeType.Polygon:
                    type = typeof(PolygonNode);
                    return true;

                case NodeType.Slice:
                    type = typeof(SliceNode);
                    return true;

                case NodeType.Star:
                    type = typeof(StarNode);
                    return true;

                case NodeType.Line:
                    type = typeof(LineNode);
                    return true;

                case NodeType.Text:
                    type = typeof(TextNode);
                    return true;

                case NodeType.Vector:
                    type = typeof(VectorNode);
                    return true;
                
                default:
                    type = typeof(Node);
                    return true;
            }
        }
    }
}