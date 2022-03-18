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
        /// Gets the 2D position in Unity space.
        /// </summary>
        public Vector2 GetPosition() => new Vector2(values[0][2], -values[1][2]);

        /// <summary>
        /// Gets the 2D scale.
        /// </summary>
        public Vector2 GetScale() =>
            // There is no scale in Figma. It's only a horizontal/vertical mirror.
            new Vector2(values[0][0] < 0 ? -1 : 1, values[1][1] < 0 ? -1 : 1);
        
        /// <summary>
        /// Gets the 2D rotation as <see cref="Quaternion"/>.
        /// </summary>
        public Quaternion GetRotation() => Quaternion.Euler(0, 0, -GetRotationAngle());
        
        /// <summary>
        /// Gets 2D rotation angle in degrees.
        /// </summary>
        public float GetRotationAngle() => Mathf.Rad2Deg * Mathf.Atan2(values[1][0], values[0][0]);
    }
}