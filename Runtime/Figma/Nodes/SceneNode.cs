using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class SceneNode : Node
    {
        /// <summary>
        /// Whether the node is visible or not.
        /// </summary>
        /// <remarks>
        /// The value that this property returns is independent from the node's parent. i.e.
        /// <list type="bullet">
        /// <item>The node isn't necessarily visible if this is <c>.visible == true</c>.</item>
        /// <item>The node isn't necessarily invisible if this is <c>.visible == false</c>.</item>
        /// <item>An object is visible if <c>.visible == true</c> for itself and all its parents.</item>
        /// </list>
        /// </remarks>
        [DataMember(Name = "visible")]
        public bool visible { get; set; } = true;
        
        /// <summary>
        /// Whether the node is locked or not, preventing certain user interactions on the canvas such as selecting
        /// and dragging. Does not affect a plugin's ability to write to those properties.
        /// </summary>
        /// <remarks>
        /// The value that this property returns is independent from the node's parent. i.e.
        /// <list type="bullet">
        /// <item>The node isn't necessarily locked if this is <c>.locked == true</c>.</item>
        /// <item>The node isn't necessarily unlocked if this is <c>.locked == false</c>.</item>
        /// <item>An object is locked if <c>.locked == true</c> for itself or any of its parents.</item>
        /// </list>
        /// </remarks>
        [DataMember(Name = "locked")]
        public bool locked { get; set; } = true;
    }
}