namespace Cdm.Figma.UI
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public static NodeObject Convert(GroupNode node, NodeConvertArgs args)
        {
            var nodeObject = NodeObject.NewNodeObject(node, args);

            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    if (args.importer.TryConvertNode(child, args, out var childNode))
                    {
                        childNode.transform.parent = nodeObject.transform;
                    }
                }
            }
            
            return nodeObject;
        }
        
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return Convert((GroupNode) node, args);
        }
    }
}