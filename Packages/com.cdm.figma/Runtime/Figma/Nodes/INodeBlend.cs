using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    public interface INodeBlend
    {
        /// <summary>
        /// Opacity of the node.
        /// </summary>
        public float opacity { get; set; }

        /// <summary>
        /// Does this node mask sibling nodes in front of it?
        /// </summary>
        public bool isMask { get; set; }
        
        /// <summary>
        /// How this node blends with nodes behind it in the scene.
        /// </summary>
        public BlendMode? blendMode { get; set; }

        /// <summary>
        /// A list of effects attached to this node.
        /// </summary>
        public List<Effect> effects { get; set; }
    }
}