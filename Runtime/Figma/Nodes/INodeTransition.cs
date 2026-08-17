namespace Cdm.Figma
{
    public interface INodeTransition
    {
        /// <summary>
        /// Node ID of node to transition to in prototyping.
        /// </summary>
        public string transitionNodeId { get; set; }

        /// <summary>
        /// The duration of the prototyping transition on this node (in milliseconds).
        /// </summary>
        public float? transitionDuration { get; set; }

        /// <summary>
        /// The easing curve used in the prototyping transition on this node.
        /// </summary>
        public EasingType? transitionEasing { get; set; }
    }
}