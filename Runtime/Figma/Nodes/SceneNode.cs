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

        /// <summary>
        /// This property is applicable only for direct children of auto-layout frames. Determines whether a
        /// layer's size and position should be determined by auto-layout settings or manually adjustable.
        /// <list type="bullet">
        /// <item>
        /// The default value of <see cref="LayoutPositioning.Auto"/> will layout this child according to auto-layout rules.
        /// </item>
        /// <item>
        /// Setting <see cref="LayoutPositioning.Absolute"/> will take this child out of auto-layout flow,
        /// while still nesting inside the auto-layout frame. This allows explicitly setting x, y, width, and height.
        /// <see cref="LayoutPositioning.Absolute"/> positioned nodes respect constraint settings.
        /// </item>
        /// </list>
        /// </summary>
        [DataMember(Name = "layoutPositioning")]
        public LayoutPositioning layoutPositioning { get; set; } = LayoutPositioning.Auto;
    }
}