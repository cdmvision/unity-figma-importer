using System;
using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.UI.Styles;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public static class FigmaNodeExtensions
    {
        /// <summary>
        /// Sets pivot, position, rotation and scale using <paramref name="nodeTransform"/>.
        /// </summary>
        /// <seealso cref="SetPivot"/>
        /// <seealso cref="SetPosition"/>
        /// <seealso cref="SetRotation"/>
        /// <seealso cref="SetScale"/>
        /// <seealso cref="SetScale"/>
        public static void SetTransform(this FigmaNode nodeObject, INodeTransform nodeTransform)
        {
            nodeObject.SetPivot();
            nodeObject.SetPosition(nodeTransform);
            nodeObject.SetRotation(nodeTransform);
            nodeObject.SetScale(nodeTransform);
            nodeObject.SetSize(nodeTransform);
        }

        /// <summary>
        /// Sets the pivot of the figma node.
        /// </summary>
        public static void SetPivot(this FigmaNode nodeObject)
        {
            // Pivot location is located at top left corner in Figma.
            nodeObject.rectTransform.pivot = new Vector2(0f, 1f);
        }

        /// <summary>
        /// Sets the rotation of the figma node.
        /// </summary>
        public static void SetPosition(this FigmaNode nodeObject, INodeTransform nodeTransform)
        {
            nodeObject.rectTransform.position = nodeTransform.GetPosition();
        }

        /// <summary>
        /// Sets the rotation of the figma node.
        /// </summary>
        public static void SetRotation(this FigmaNode nodeObject, INodeTransform nodeTransform)
        {
            nodeObject.rectTransform.rotation = nodeTransform.GetRotation();
        }

        /// <summary>
        /// Sets the scale of the figma node.
        /// </summary>
        /// <remarks>
        /// In Figma, there is no 'scaling'. But vertical/horizontal mirror information is stored as scale in
        /// <see cref="INodeTransform.relativeTransform"/>.
        /// </remarks>
        public static void SetScale(this FigmaNode nodeObject, INodeTransform nodeTransform)
        {
            var scale = nodeTransform.GetScale();
            nodeObject.rectTransform.localScale = new Vector3(scale.x, scale.y, 1);
        }

        public static void SetSize(this FigmaNode nodeObject, INodeTransform nodeTransform)
        {
            nodeObject.rectTransform.sizeDelta = nodeTransform.size;
        }

        public static void ApplyStyles(this FigmaNode nodeObject)
        {
            foreach (var style in nodeObject.styles)
            {
                style.SetStyle(nodeObject.gameObject, new StyleArgs("", true));
            }
        }

        public static void ApplyStylesSelectors(this FigmaNode nodeObject)
        {
            foreach (var style in nodeObject.styles)
            {
                style.SetStyleAsSelector(nodeObject.gameObject, new StyleArgs("", true));
            }
        }

        /// <summary>
        /// Traverse nodes by using depth first search from starting node given.
        /// </summary>
        public static void TraverseDfs(this FigmaNode node, Func<FigmaNode, bool> handler)
        {
            if (handler(node))
            {
                foreach (Transform child in node.transform)
                {
                    var childObject = child.GetComponent<FigmaNode>();
                    if (childObject != null)
                    {
                        childObject.TraverseDfs(handler);
                    }
                }
            }
        }

        /// <summary>
        /// Traverse nodes by using breadth first search from starting node given.
        /// </summary>
        public static void TraverseBfs(this FigmaNode node, Func<FigmaNode, bool> handler)
        {
            if (handler(node))
            {
                node.TraverseBfsInternal(handler);
            }
        }
        
        private static void TraverseBfsInternal(this FigmaNode node, Func<FigmaNode, bool> handler)
        {
            var children = new List<FigmaNode>();
            foreach (Transform child in node.transform)
            {
                var childObject = child.GetComponent<FigmaNode>();
                if (childObject != null)
                {
                    if (handler(childObject))
                    {
                        children.Add(childObject);
                    }
                }
            }
            
            foreach (var child in children)
            {
                child.TraverseBfsInternal(handler);
            }
        }

        public static void TraverseUp(this FigmaNode node, Func<FigmaNode, bool> handler)
        {
            for (var current = node.transform; current != null; current = current.transform.parent)
            {
                var currentNode = current.GetComponent<FigmaNode>();
                if (currentNode != null)
                {
                    if (!handler(currentNode))
                    {
                        break;
                    }
                }
            }
        }

        public static FigmaNode Find(this FigmaNode node, Func<FigmaNode, bool> handler)
        {
            FigmaNode target = null;
            node.TraverseDfs(x =>
            {
                if (handler(x))
                {
                    target = x;
                    return false;
                }

                return true;
            });

            return target;
        }

        public static FigmaNode Find(this FigmaNode node, string nodeId)
        {
            return node.Find(n => n.nodeId == nodeId);
        }

        public static FigmaNode Query(this FigmaNode node, string bindingKey)
        {
            return node.Query<FigmaNode>(bindingKey);
        }

        public static FigmaNode[] QueryAll(this FigmaNode node, string bindingKey)
        {
            return node.QueryAll<FigmaNode>(bindingKey);
        }

        public static T Query<T>(this FigmaNode node, string bindingKey) where T : UnityEngine.Component
        {
            return (T)node.Query(bindingKey, typeof(T));
        }

        public static T[] QueryAll<T>(this FigmaNode node, string bindingKey) where T : UnityEngine.Component
        {
            return node.QueryAll(bindingKey, typeof(T)).Cast<T>().ToArray();
        }

        public static UnityEngine.Component Query(this FigmaNode node, string bindingKey, Type type)
        {
            if (node.figmaDesign != null)
            {
                return node.QueryCached(bindingKey, type);
            }

            // Search every child using breadth first search.
            UnityEngine.Component target = null;
                
            node.TraverseBfs(n =>
            {
                if (n.bindingKey == bindingKey)
                {
                    var component = n.GetComponent(type);
                    if (component != null)
                    {
                        target = component;
                        return false;
                    }
                }
                    
                return true;
            });

            return target;
        }

        private static UnityEngine.Component QueryCached(this FigmaNode node, string bindingKey, Type type)
        {
            var bindings = node.figmaDesign.bindings.Where(binding => binding.key == bindingKey);

            var minDistance = float.MaxValue;
            UnityEngine.Component closestComponent = null;

            foreach (var binding in bindings)
            {
                var tokens = binding.path.Split(Binding.PathSeparator);

                var index = Array.IndexOf(tokens, node.nodeId);
                if (index >= 0)
                {
                    var distance = tokens.Length - index - 1;
                    if (minDistance > distance)
                    {
                        var targetNode = GetNodeFromPath(node, tokens, index);
                        
                        var component = targetNode.GetComponent(type);
                        if (component != null)
                        {
                            minDistance = distance;
                            closestComponent = component;
                        }
                    }
                }
            }

            return closestComponent;
        }

        private static FigmaNode GetNodeFromPath(FigmaNode figmaNode, string[] path, int start)
        {
            FigmaNode target = null;
            var i = start;
            
            figmaNode.TraverseBfs(node =>
            {
                if (node.nodeId == path[i])
                {
                    if (i == path.Length - 1)
                    {
                        target = node;
                        return false;
                    }
                    
                    i++;
                    return true;
                }

                return false;
            });

            return target;
        }

        public static UnityEngine.Component[] QueryAll(this FigmaNode node, string bindingKey, Type type)
        {
            var bindings = node.figmaDesign.bindings.Where(binding => binding.key == bindingKey);

            var components = new List<UnityEngine.Component>();

            foreach (var binding in bindings)
            {
                var tokens = binding.path.Split(Binding.PathSeparator);

                var index = Array.IndexOf(tokens, node.nodeId);
                if (index >= 0)
                {
                    var targetNode = GetNodeFromPath(node, tokens, index);
                    var component = targetNode.GetComponent(type);
                    if (component != null)
                    {
                        components.Add(component);
                    }
                }
            }

            return components.ToArray();
        }
    }
}