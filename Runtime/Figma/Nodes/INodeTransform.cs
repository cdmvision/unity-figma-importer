﻿using UnityEngine;

namespace Cdm.Figma
{
    public interface INodeTransform
    {
        /// <summary>
        /// Width and height of element. This is different from the width and height of the bounding box in that the
        /// absolute bounding box represents the element after scaling and rotation.
        /// </summary>
        public Vector size { get; set; }
        
        /// <summary>
        /// Bounding box of the node in absolute space coordinates.
        /// </summary>
        public Rectangle absoluteBoundingBox { get; set; }
        
        /// <summary>
        /// The top two rows of a matrix that represents the 2D transform of this node relative to its parent.
        /// The bottom row of the matrix is implicitly always (0, 0, 1). Use to transform coordinates in geometry.
        /// Only present if geometry=paths is passed.
        /// </summary>
        public AffineTransform relativeTransform { get; set; }
        
        /// <inheritdoc cref="AffineTransform.GetPosition"/>
        public Vector2 GetPosition() => relativeTransform.GetPosition();

        /// <inheritdoc cref="AffineTransform.GetScale"/>
        public Vector2 GetScale() => relativeTransform.GetScale();

        /// <inheritdoc cref="AffineTransform.GetRotation"/>
        public Quaternion GetRotation() => relativeTransform.GetRotation();

        /// <inheritdoc cref="AffineTransform.GetRotationAngle"/>
        public float GetRotationAngle() => relativeTransform.GetRotationAngle();
    }
}