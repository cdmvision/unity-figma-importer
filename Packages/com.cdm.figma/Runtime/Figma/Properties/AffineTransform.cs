using System.Runtime.Serialization;
using UnityEngine;

namespace Cdm.Figma
{
    /// <summary>
    /// A 2x3 affine transformation matrix.
    ///
    /// A 2D affine transformation matrix that can be used to calculate the affine transforms applied to a layer,
    /// including scaling, rotation, shearing, and translation.
    ///
    /// The form of the matrix is given as an array of 2 arrays of 3 numbers each.
    /// E.g. the identity matrix would be [[1, 0, 0], [0, 1, 0]]
    /// </summary>
    [DataContract]
    public class AffineTransform
    {
        public float[][] values { get; set; }

        public AffineTransform()
        {
        }

        public AffineTransform(float[][] values)
        {
            this.values = values;
        }

        /// <summary>
        /// Gets 2D position in Unity space.
        /// </summary>
        public Vector2 GetPosition() => new Vector2(values[0][2], -values[1][2]);

        /// <summary>
        /// Gets 2D rotation in degrees.
        /// </summary>
        public float GetRotation() => Mathf.Rad2Deg * Mathf.Atan2(values[0][0], values[1][0]);
    }
}