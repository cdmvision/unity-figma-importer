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

        public static string GetBindingKey(this Node node)
        {
            if (node.TryGetPluginData(out var data))
            {
                return data.hasBindingKey ? data.bindingKey : null;
            }

            return null;
        }
        
        public static string GetLocalizationKey(this Node node)
        {
            if (node.TryGetPluginData(out var data))
            {
                return data.hasLocalizationKey ? data.localizationKey : null;
            }

            return null;
        }
        
        public static string GetComponentType(this Node node)
        {
            if (node.TryGetPluginData(out var data))
            {
                return data.hasComponentType ? data.componentType : null;
            }

            return null;
        }

        private static bool TryGetPluginData(this Node node, out PluginData pluginData)
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