using System;
using System.Linq;

namespace Cdm.Figma
{
    public static class NodeExtensions
    {    
        /// <summary>
        /// Traverse nodes by using depth first search from starting node given.
        /// </summary>
        public static void TraverseDfs(this Node node, Func<Node, bool> handler)
        {
            if (handler(node))
            {
                var children = node.GetChildren();
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        child.TraverseDfs(handler);
                    }
                }
            }
        }

        /// <summary>
        /// Traverse nodes specified types by using depth first search from starting node.
        /// </summary>
        public static void TraverseDfs(this Node node, Func<Node, bool> handler, params string[] nodeTypes)
        {
            node.TraverseDfs(n =>
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
        
        public static void TraverseUp(this Node node, Func<Node, bool> handler)
        {
            for (var current = node; current != null; current = current.parent)
            {
                if (!handler(current))
                {
                    break;
                }
            }
        }

        public static Node Find(this Node node, string nodeId)
        {
            Node target = null;
            node.TraverseDfs(n =>
            {
                if (n.id == nodeId)
                {
                    target = n;
                    return false;
                }

                return true;
            });

            return target;
        }

        public static Node Find(this Node node, string nodeId, params string[] nodeTypes)
        {
            Node target = null;
            node.TraverseDfs(n =>
            {
                if (n.id == nodeId)
                {
                    target = n;
                    return false;
                }

                return true;
            }, nodeTypes);

            return target;
        }

        /// <summary>
        /// Gets binding key from node's plugin data.
        /// </summary>
        public static string GetBindingKey(this Node node)
        {
            if (node.TryGetPluginData(out var data))
            {
                return data.hasBindingKey ? data.bindingKey : null;
            }

            return null;
        }
        
        /// <summary>
        /// Gets localization key from node's plugin data.
        /// </summary>
        public static string GetLocalizationKey(this TextNode node)
        {
            if (node.TryGetPluginData(out var data))
            {
                return data.hasLocalizationKey ? data.localizationKey : null;
            }

            return null;
        }

        /// <summary>
        /// Gets component type from node's plugin data.
        /// </summary>
        public static string GetComponentType(this Node node)
        {
            if (node.TryGetPluginData(out var data))
            {
                return data.hasComponentType ? data.componentType : null;
            }

            return null;
        }
        
        /// <summary>
        /// Gets tags from node's plugin data.
        /// </summary>
        public static string[] GetTags(this Node node)
        {
            if (node.TryGetPluginData(out var data))
            {
                if (data.hasTags)
                {
                    var tags = data.tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    
                    for (var i = 0; i < tags.Length; i++)
                    {
                        tags[i] = tags[i].Trim();
                    }

                    return tags;
                }
            }

            return Array.Empty<string>();
        }

        public static bool TryGetPluginData(this Node node, out PluginData pluginData)
        {
            if (node.pluginData != null && node.pluginData.TryGetValue(PluginData.Id, out var data))
            {
                pluginData = PluginData.FromJson(data);
                return pluginData != null;
            }

            pluginData = null;
            return false;
        }
    }
}