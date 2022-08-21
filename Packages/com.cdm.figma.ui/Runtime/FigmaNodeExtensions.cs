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

        public static void Traverse(this FigmaNode node, Func<FigmaNode, bool> handler)
        {
            if (handler(node))
            {
                foreach (Transform child in node.transform)
                {
                    var childObject = child.GetComponent<FigmaNode>();
                    if (childObject != null)
                    {
                        childObject.Traverse(handler);
                    }
                }
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
        
        public static FigmaNode Find(this FigmaNode node, string nodeID)
        {
            return node.Find(n => n.nodeID == nodeID);
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
            return (T) node.Query(bindingKey, typeof(T));
        }

        public static UnityEngine.Component Query(this FigmaNode node, string bindingKey, Type type)
        {
            UnityEngine.Component component = null;
            node.Traverse(n =>
            {
                if (n.bindingKey == bindingKey)
                {
                    component = n.GetComponent(type);
                    if (component != null)
                    {
                        return false;
                    }
                }

                return true;
            });

            return component;
        }

        public static T[] QueryAll<T>(this FigmaNode node, string bindingKey) where T : UnityEngine.Component
        {
            return node.QueryAll(bindingKey, typeof(T)).Cast<T>().ToArray();
        }
        
        public static UnityEngine.Component[] QueryAll(this FigmaNode node, string bindingKey, Type type)
        {
            var components = new List<UnityEngine.Component>();
            node.Traverse(n =>
            {
                if (n.bindingKey == bindingKey)
                {
                    var component = n.GetComponent(type);
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