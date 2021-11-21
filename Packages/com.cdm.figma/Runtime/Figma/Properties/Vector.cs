using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Cdm.Figma
{
    /// <summary>
    /// A 2d vector
    ///
    /// This field contains three vectors, each of which are a position in
    /// normalized object space (normalized object space is if the top left
    /// corner of the bounding box of the object is (0, 0) and the bottom
    /// right is (1,1)). The first position corresponds to the start of the
    /// gradient (value 0 for the purposes of calculating gradient stops),
    /// the second position is the end of the gradient (value 1), and the
    /// third handle position determines the width of the gradient (only
    /// relevant for non-linear gradients).
    ///
    /// 2d vector offset within the frame.
    /// </summary>
    [Serializable]
    public class Vector
    {
        /// <summary>
        /// X coordinate of the vector
        /// </summary>
        [JsonProperty("x")]
        public float x { get; set; }

        /// <summary>
        /// Y coordinate of the vector
        /// </summary>
        [JsonProperty("y")]
        public float y { get; set; }

        public Vector()
        {
        }
        
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        
        public static implicit operator Vector2(Vector v) => new Vector2(v.x, v.y);
        public static explicit operator Vector(Vector2 v) => new Vector(v.x, v.y);
    }
}