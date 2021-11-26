using System;
using System.Linq;

namespace Cdm.Figma
{
    public static class NodeExtensions
    {
        public static void Traverse(this Node node, Func<Node, bool> handler)
        {
            if (handler(node))
            {
                var children = node.GetChildren();
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        child.Traverse(handler);
                    }    
                }
            }
        }

        public static void Traverse(this Node node, Func<Node, bool> handler, params string[] nodeTypes)
        {
            node.Traverse(n =>
            {
                if (nodeTypes.Contains(n.type))
                {
                    if (!handler(n))
                    {
                        return false;
                    }
                }
                
                return true;
            });
        }
    }
}