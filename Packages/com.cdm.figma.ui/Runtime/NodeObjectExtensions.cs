using System;
using System.Collections.Generic;
using Cdm.Figma.UI.Styles;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public static class NodeObjectExtensions
    {
        /// <summary>
        /// Sets pivot, position, rotation and scale using <paramref name="nodeTransform"/>.
        /// </summary>
        /// <seealso cref="SetPivot"/>
        /// <seealso cref="SetPosition"/>
        /// <seealso cref="SetRotation"/>
        /// <seealso cref="SetScale"/>
        /// <seealso cref="SetScale"/>
        public static void SetTransform(this NodeObject nodeObject, INodeTransform nodeTransform)
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
        public static void SetPivot(this NodeObject nodeObject)
        {
            // Pivot location is located at top left corner in Figma.
            nodeObject.rectTransform.pivot = new Vector2(0f, 1f);
        }

        /// <summary>
        /// Sets the rotation of the figma node.
        /// </summary>
        public static void SetPosition(this NodeObject nodeObject, INodeTransform nodeTransform)
        {
            nodeObject.rectTransform.position = nodeTransform.GetPosition();
        }

        /// <summary>
        /// Sets the rotation of the figma node.
        /// </summary>
        public static void SetRotation(this NodeObject nodeObject, INodeTransform nodeTransform)
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
        public static void SetScale(this NodeObject nodeObject, INodeTransform nodeTransform)
        {
            var scale = nodeTransform.GetScale();
            nodeObject.rectTransform.localScale = new Vector3(scale.x, scale.y, 1);
        }

        public static void SetSize(this NodeObject nodeObject, INodeTransform nodeTransform)
        {
            nodeObject.rectTransform.sizeDelta = nodeTransform.size;
        }

        public static void ApplyStyles(this NodeObject nodeObject)
        {
            foreach (var style in nodeObject.styles)
            {
                style.SetStyle(nodeObject.gameObject, new StyleArgs("", true));
            }
        }

        public static void ApplyStylesSelectors(this NodeObject nodeObject)
        {
            foreach (var style in nodeObject.styles)
            {
                style.SetStyleAsSelector(nodeObject.gameObject, new StyleArgs("", true));
            }
        }

        public static void Traverse(this NodeObject node, Func<NodeObject, bool> handler)
        {
            if (handler(node))
            {
                foreach (Transform child in node.transform)
                {
                    var childObject = child.GetComponent<NodeObject>();
                    if (childObject != null)
                    {
                        childObject.Traverse(handler);
                    }
                }
            }
        }

        public static void TraverseUp(this NodeObject node, Func<NodeObject, bool> handler)
        {
            for (var current = node.transform; current != null; current = current.transform.parent)
            {
                var currentNode = current.GetComponent<NodeObject>();
                if (currentNode != null)
                {
                    if (!handler(currentNode))
                    {
                        break;
                    }
                }
            }
        }

        public static NodeObject Find(this NodeObject node, Func<NodeObject, bool> handler)
        {
            NodeObject target = null;
            node.Traverse(x =>
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
        
        public static NodeObject Find(this NodeObject node, string nodeID)
        {
            return node.Find(n => n.nodeID == nodeID);
        }
        
        public static NodeObject Query(this NodeObject node, string bindingKey)
        {
            return node.Query<NodeObject>(bindingKey);
        }
        
        public static NodeObject[] QueryAll(this NodeObject node, string bindingKey)
        {
            return node.QueryAll<NodeObject>(bindingKey);
        }
        
        public static T Query<T>(this NodeObject node, string bindingKey) where T : UnityEngine.Component
        {
            T component = null;
            node.Traverse(n =>
            {
                if (n.bindingKey == bindingKey)
                {
                    component = n.GetComponent<T>();
                    if (component != null)
                    {
                        return false;
                    }
                }

                return true;
            });

            return component;
        }
        
        public static T[] QueryAll<T>(this NodeObject node, string bindingKey) where T : UnityEngine.Component
        {
            var components = new List<T>();
            node.Traverse(n =>
            {
                if (n.bindingKey == bindingKey)
                {
                    var component = n.GetComponent<T>();
                    if (component != null)
                    {
                        components.Add(component);
                    }
                }

                return true;
            });

            return components.ToArray();
        }
    }
}