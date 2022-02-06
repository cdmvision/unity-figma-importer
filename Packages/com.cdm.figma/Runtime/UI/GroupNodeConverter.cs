namespace Cdm.Figma.UI
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public static NodeObject Convert(NodeObject parentObject, GroupNode groupNode, NodeConvertArgs args)
        {
            var groupNodeObject = NodeObject.NewNodeObject(groupNode, args);
            
            var children = groupNode.children;
            if (children != null)
            {
                for (var child = 0; child < children.Length; child++)
                {
                    if (args.importer.TryConvertNode(groupNodeObject, children[child], args, out var childElement))
                    {
                        // TODO:

                        if (childElement != groupNodeObject)
                        {
                            childElement.rectTransform.SetParent(groupNodeObject.rectTransform);
                        }
                    }
                }
            }

            return groupNodeObject;
        }
        
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (GroupNode) node, args);
        }
    }
}