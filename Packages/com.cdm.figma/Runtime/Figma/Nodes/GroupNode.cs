using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class GroupNode : Node
    {
        public override NodeType type => NodeType.Group;
        
        /// <summary>
        /// A list of nodes that are direct children of this node.
        /// </summary>
        [JsonProperty("children")]
        public Node[] children { get; set; }

        /// <summary>
        /// If true, layer is locked and cannot be edited.
        /// </summary>
        [JsonProperty("locked")]
        public bool locked { get; private set; } = false;

        /// <summary>
        /// An array of fill paints applied to the node.
        /// </summary>
        [JsonProperty("fills")]
        public List<Paint> fills { get; private set; } = new List<Paint>();

        /// <summary>
        /// An array of stroke paints applied to the node.
        /// </summary>
        [JsonProperty("strokes")]
        public List<Paint> strokes { get; private set; } = new List<Paint>();

        /// <summary>
        /// The weight of strokes on the node.
        /// </summary>
        [JsonProperty("strokeWeight")]
        public float? strokeWeight { get; set; }

        /// <summary>
        /// Position of stroke relative to vector outline.
        /// </summary>
        [JsonProperty("strokeAlign")]
        public StrokeAlign? strokeAlign { get; set; }

        /// <summary>
        /// Radius of each corner of the frame if a single radius is set for all corners.
        /// </summary>
        [JsonProperty("cornerRadius")]
        public float? cornerRadius { get; set; }

        /// <summary>
        /// Array of length 4 of the radius of each corner of the frame, starting in the top left and
        /// proceeding clockwise.
        /// </summary>
        [JsonProperty("rectangleCornerRadii")]
        public float[] rectangleCornerRadii { get; set; }

        /// <summary>
        /// A list of export settings representing images to export from the canvas.
        /// </summary>
        [JsonProperty("exportSettings")]
        public List<ExportSetting> exportSettings { get; private set; } = new List<ExportSetting>();

        /// <summary>
        /// How this node blends with nodes behind it in the scene.
        /// </summary>
        [JsonProperty("blendMode")]
        public BlendMode? blendMode { get; set; }

        /// <summary>
        /// Keep height and width constrained to same ratio.
        /// </summary>
        [JsonProperty("preserveRatio")]
        public bool preserveRatio { get; set; } = false;

        /// <summary>
        /// Horizontal and vertical layout constraints for node.
        /// </summary>
        [JsonProperty("constraints")]
        public LayoutConstraint constraints { get; set; }

        /// <summary>
        /// Determines if the layer should stretch along the parent’s counter axis. This property is only provided for
        /// direct children of auto-layout frames.
        /// </summary>
        [JsonProperty("layoutAlign")]
        public LayoutAlign layoutAlign { get; set; }

        /// <summary>
        /// Node ID of node to transition to in prototyping.
        /// </summary>
        [JsonProperty("transitionNodeID")]
        public string transitionNodeId { get; set; } = null;

        /// <summary>
        /// The duration of the prototyping transition on this node (in milliseconds).
        /// </summary>
        [JsonProperty("transitionDuration")]
        public float? transitionDuration { get; set; } = null;

        /// <summary>
        /// The easing curve used in the prototyping transition on this node.
        /// </summary>
        [JsonProperty("transitionEasing")]
        public EasingType? transitionEasing { get; set; } = null;

        /// <summary>
        /// Opacity of the node.
        /// </summary>
        [JsonProperty("opacity")]
        public float opacity { get; set; } = 1f;

        /// <summary>
        /// Bounding box of the node in absolute space coordinates.
        /// </summary>
        [JsonProperty("absoluteBoundingBox")]
        public Rectangle absoluteBoundingBox { get; set; }

        /// <summary>
        /// Width and height of element. This is different from the width and height of the bounding box in that the
        /// absolute bounding box represents the element after scaling and rotation.
        ///
        /// Only present if geometry=paths is passed.
        /// </summary>
        [JsonProperty("size")]
        public Vector size { get; set; }

        /// <summary>
        /// The top two rows of a matrix that represents the 2D transform of this node relative to its parent.
        /// The bottom row of the matrix is implicitly always (0, 0, 1). Use to transform coordinates in geometry.
        /// Only present if geometry=paths is passed.
        /// </summary>
        [JsonProperty("relativeTransform")]
        public AffineTransform relativeTransform { get; set; }

        /// <summary>
        /// Whether or not this node clip content outside of its bounds.
        /// </summary>
        [JsonProperty("clipsContent")]
        public bool clipsContent { get; set; } = false;

        /// <summary>
        /// Whether this layer uses auto-layout to position its children.
        /// </summary>
        [JsonProperty("layoutMode")]
        public LayoutMode layoutMode { get; set; }

        /// <summary>
        /// Whether the primary axis has a fixed length (determined by the user) or an automatic length
        /// (determined by the layout engine). This property is only applicable for auto-layout frames.
        /// </summary>
        [JsonProperty("primaryAxisSizingMode")]
        public AxisSizingMode primaryAxisSizingMode { get; set; } = AxisSizingMode.Auto;

        /// <summary>
        /// Whether the counter axis has a fixed length (determined by the user) or an automatic length
        /// (determined by the layout engine). This property is only applicable for auto-layout frames.
        /// </summary>
        [JsonProperty("counterAxisSizingMode")]
        public AxisSizingMode counterAxisSizingMode { get; set; } = AxisSizingMode.Auto;

        /// <summary>
        /// Determines how the auto-layout frame’s children should be aligned in the primary axis direction.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [JsonProperty("primaryAxisAlignItems")]
        public PrimaryAxisAlignItems primaryAxisAlignItems { get; set; } = PrimaryAxisAlignItems.Min;

        /// <summary>
        /// Determines how the auto-layout frame’s children should be aligned in the counter axis direction.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [JsonProperty("counterAxisAlignItems")]
        public CounterAxisAlignItems counterAxisAlignItems { get; set; } = CounterAxisAlignItems.Min;

        /// <summary>
        /// The padding between the left border of the frame and its children.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [JsonProperty("paddingLeft")]
        public float paddingLeft { get; set; } = 0f;

        /// <summary>
        /// The padding between the right border of the frame and its children.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [JsonProperty("paddingRight")]
        public float paddingRight { get; set; } = 0f;

        /// <summary>
        /// The padding between the top border of the frame and its children.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [JsonProperty("paddingTop")]
        public float paddingTop { get; set; } = 0f;

        /// <summary>
        /// The padding between the bottom border of the frame and its children.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [JsonProperty("paddingBottom")]
        public float paddingBottom { get; set; } = 0f;

        /// <summary>
        /// The distance between children of the frame. This property is only applicable for auto-layout frames.
        /// </summary>
        [JsonProperty("itemSpacing")]
        public float itemSpacing { get; set; } = 0f;

        /// <summary>
        /// Defines the scrolling behavior of the frame, if there exist contents outside of the frame boundaries.
        /// The frame can either scroll vertically, horizontally, or in both directions to the extents of the content
        /// contained within it. This behavior can be observed in a prototype.
        /// </summary>
        [JsonProperty("layoutGrids")]
        public OverflowDirection overflowDirection { get; set; } = OverflowDirection.None;
        
        /// <summary>
        /// A list of effects attached to this node.
        /// </summary>
        [JsonProperty("effects")]
        public List<Effect> effects { get; private set; } = new List<Effect>();
        
        /// <summary>
        /// Does this node mask sibling nodes in front of it?
        /// </summary>
        [JsonProperty("isMask")]
        public bool isMask { get; set; } = false;
        
        /// <summary>
        /// Does this mask ignore fill style (like gradients) and effects?
        /// </summary>
        [JsonProperty("isMaskOutline")]
        public bool isMaskOutline { get; set; } = false;
        
        public override Node[] GetChildren() => children;
    }

    [Serializable]
    public enum OverflowDirection
    {
        [EnumMember(Value = "NONE")]
        None,
        
        [EnumMember(Value = "HORIZONTAL_SCROLLING")]
        HorizontalScrolling,
        
        [EnumMember(Value = "VERTICAL_SCROLLING")]
        VerticalScrolling,
        
        [EnumMember(Value = "HORIZONTAL_AND_VERTICAL_SCROLLING")]
        HorizontalAndVerticalScrolling
    }

    [Serializable]
    public enum PrimaryAxisAlignItems
    {
        [EnumMember(Value = "MIN")]
        Min,

        [EnumMember(Value = "CENTER")]
        Center,

        [EnumMember(Value = "MAX")]
        Max,

        [EnumMember(Value = "SPACE_BETWEEN")]
        SpaceBetween
    }

    [Serializable]
    public enum CounterAxisAlignItems
    {
        [EnumMember(Value = "MIN")]
        Min,

        [EnumMember(Value = "CENTER")]
        Center,

        [EnumMember(Value = "MAX")]
        Max
    }

    [Serializable]
    public enum AxisSizingMode
    {
        [EnumMember(Value = "AUTO")]
        Auto,

        [EnumMember(Value = "FIXED")]
        Fixed
    }

    [Serializable]
    public enum LayoutMode
    {
        [EnumMember(Value = "NONE")]
        None,

        [EnumMember(Value = "HORIZONTAL")]
        Horizontal,

        [EnumMember(Value = "VERTICAL")]
        Vertical,
    }

    /// <summary>
    /// In horizontal auto-layout frames, "MIN" and "MAX" correspond to "TOP" and "BOTTOM".
    /// In vertical auto-layout frames, "MIN" and "MAX" correspond to "LEFT" and "RIGHT".
    /// </summary>
    [Serializable]
    public enum LayoutAlign
    {
        [EnumMember(Value = "INHERIT")]
        Inherit,

        [EnumMember(Value = "STRETCH")]
        Stretch,

        [EnumMember(Value = "MIN")]
        Min,

        [EnumMember(Value = "CENTER")]
        Center,

        [EnumMember(Value = "MAX")]
        Max,
    }

    [Serializable]
    public enum StrokeAlign
    {
        /// <summary>
        /// Draw stroke centered along the shape boundary.
        /// </summary>
        [EnumMember(Value = "CENTER")]
        Center,

        /// <summary>
        /// Draw stroke inside the shape boundary.
        /// </summary>
        [EnumMember(Value = "INSIDE")]
        Inside,

        /// <summary>
        /// Draw stroke outside the shape boundary.
        /// </summary>
        [EnumMember(Value = "OUTSIDE")]
        Outside
    }
}