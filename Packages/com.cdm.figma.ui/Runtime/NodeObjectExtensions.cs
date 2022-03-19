using System;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public enum TransformType
    {
        Relative,
        Absolute
    }
    
    public static class NodeObjectExtensions
    {
        /// <summary>
        /// Sets pivot, position, rotation and scale using <paramref name="nodeTransform"/>.
        /// </summary>
        /// <seealso cref="SetPivot"/>
        /// <seealso cref="SetPosition"/>
        /// <seealso cref="SetRotation"/>
        /// <seealso cref="SetScale"/>
        public static void SetTransform(this NodeObject nodeObject, INodeTransform nodeTransform, TransformType type)
        {
            nodeObject.SetPivot();
            nodeObject.SetPosition(nodeTransform, type);

            if (type == TransformType.Relative)
            {
                nodeObject.SetRotation(nodeTransform);
                nodeObject.SetScale(nodeTransform);
            }
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
        /// Sets the position of the figma node.
        /// </summary>
        public static void SetPosition(this NodeObject nodeObject, INodeTransform nodeTransform, TransformType type)
        {
            switch (type)
            {
                case TransformType.Relative:
                    nodeObject.rectTransform.position = nodeTransform.relativeTransform.GetPosition();
                    break;
                case TransformType.Absolute:
                    nodeObject.rectTransform.position = 
                        new Vector3(nodeTransform.absoluteBoundingBox.x, -nodeTransform.absoluteBoundingBox.y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        /// <summary>
        /// Sets the rotation of the figma node.
        /// </summary>
        public static void SetRotation(this NodeObject nodeObject, INodeTransform nodeTransform)
        {
            nodeObject.rectTransform.rotation = nodeTransform.relativeTransform.GetRotation();
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
            var scale = nodeTransform.relativeTransform.GetScale();
            nodeObject.rectTransform.localScale = new Vector3(scale.x, scale.y, 1);
        }
        
        /// <summary>
        /// Sets the size of the figma node.
        /// </summary>
        public static void SetSize(this NodeObject nodeObject, INodeTransform nodeTransform, TransformType type)
        {
            switch (type)
            {
                case TransformType.Relative:
                    nodeObject.rectTransform.sizeDelta = new Vector2(nodeTransform.size.x, nodeTransform.size.y);
                    break;
                case TransformType.Absolute:
                    nodeObject.rectTransform.sizeDelta = 
                        new Vector2(nodeTransform.absoluteBoundingBox.width, nodeTransform.absoluteBoundingBox.height);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}