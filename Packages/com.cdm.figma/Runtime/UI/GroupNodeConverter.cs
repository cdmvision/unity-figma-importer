namespace Cdm.Figma.UI
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public static NodeObject Convert(NodeObject parentObject, GroupNode node, NodeConvertArgs args)
        {
            var nodeObject = NodeObject.NewNodeObject(node, args);

            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    if (args.importer.TryConvertNode(parentObject, child, args, out var childNode))
                    {
                        childNode.transform.parent = nodeObject.transform;
                    }
                }
            }
            
            return nodeObject;
        }
        
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (GroupNode) node, args);
        }
    }
}