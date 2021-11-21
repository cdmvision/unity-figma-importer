using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    /// <summary>
    /// A rectangle that expresses a bounding box in absolute coordinates.
    /// </summary>
    [DataContract]
    public class Rectangle
    {
        /// <summary>
        /// X coordinate of top left corner of the rectangle.
        /// </summary>
        [JsonProperty("x")]
        public float x { get; set; }

        /// <summary>
        /// Y coordinate of top left corner of the rectangle.
        /// </summary>
        [JsonProperty("y")]
        public float y { get; set; }
        
        /// <summary>
        /// Width of the rectangle
        /// </summary>
        [JsonProperty("width")]
        public float width { get; set; }
        
        /// <summary>
        /// Height of the rectangle.
        /// </summary>
        [JsonProperty("height")]
        public float height { get; set; }

        // TODO: ?
        public UnityEngine.Vector3 position => new(x, -y);
    }
}