using System;
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
    }
}