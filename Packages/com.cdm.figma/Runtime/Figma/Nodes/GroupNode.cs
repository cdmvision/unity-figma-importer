using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The group node is a container used to semantically group related nodes. You can think of them as a folder in
    /// the layers panel. It is different from FrameNode, which defines layout and is closer to a <![CDATA[<div>]]>
    /// in HTML.
    /// 
    /// Groups in Figma are always positioned and sized to fit their content. As such, while you can move or resize a
    /// group, you should also expect that a group's position and size will change if you change its content. See the
    /// <see cref="relativeTransform"/> page for more details.
    /// 
    /// In Figma, groups must always have children. A group with no children will delete itself.
    /// </summary>
    [DataContract]
    public class GroupNode
        : SceneNode, INodeTransform, INodeLayout, INodeTransition, INodeExport
    {
        public override NodeType type => NodeType.Group;

        /// <summary>
        /// A list of nodes that are direct children of this node.
        /// </summary>
        [DataMember(Name = "children")]
        public Node[] children { get; set; }
        
        /// <summary>
        /// A list of export settings representing images to export from the canvas.
        /// </summary>
        [DataMember(Name = "exportSettings")]
        public List<ExportSetting> exportSettings { get; set; } = new List<ExportSetting>();

        /// <summary>
        /// Keep height and width constrained to same ratio.
        /// </summary>
        [DataMember(Name = "preserveRatio")]
        public bool preserveRatio { get; set; }

        /// <summary>
        /// Group node does not have this property.
        /// </summary>
        [DataMember(Name = "layoutGrow")]
        public float? layoutGrow { get; set; }

        /// <summary>
        /// Horizontal and vertical layout constraints for node.
        /// </summary>
        [DataMember(Name = "constraints")]
        public LayoutConstraint constraints { get; set; }

        /// <summary>
        /// Determines if the layer should stretch along the parent’s counter axis. This property is only provided for
        /// direct children of auto-layout frames.
        /// </summary>
        [DataMember(Name = "layoutAlign")]
        public LayoutAlign layoutAlign { get; set; }
        
        [DataMember(Name = "minWidth")]
        public float? minWidth { get; set; }
        
        [DataMember(Name = "maxWidth")]
        public float? maxWidth { get; set; }
        
        [DataMember(Name = "minHeight")]
        public float? minHeight { get; set; }
        
        [DataMember(Name = "maxHeight")]
        public float? maxHeight { get; set; }

        /// <summary>
        /// Node ID of node to transition to in prototyping.
        /// </summary>
        [DataMember(Name = "transitionNodeID")]
        public string transitionNodeId { get; set; } = null;

        /// <summary>
        /// The duration of the prototyping transition on this node (in milliseconds).
        /// </summary>
        [DataMember(Name = "transitionDuration")]
        public float? transitionDuration { get; set; } = null;

        /// <summary>
        /// The easing curve used in the prototyping transition on this node.
        /// </summary>
        [DataMember(Name = "transitionEasing")]
        public EasingType? transitionEasing { get; set; } = null;

        /// <summary>
        /// Bounding box of the node in absolute space coordinates.
        /// </summary>
        [DataMember(Name = "absoluteBoundingBox")]
        public Rectangle absoluteBoundingBox { get; set; }

        /// <summary>
        /// Width and height of element. This is different from the width and height of the bounding box in that the
        /// absolute bounding box represents the element after scaling and rotation.
        ///
        /// Only present if geometry=paths is passed.
        /// </summary>
        [DataMember(Name = "size")]
        public Vector size { get; set; }

        /// <summary>
        /// The top two rows of a matrix that represents the 2D transform of this node relative to its parent.
        /// The bottom row of the matrix is implicitly always (0, 0, 1). Use to transform coordinates in geometry.
        /// Only present if geometry=paths is passed.
        /// </summary>
        [DataMember(Name = "relativeTransform")]
        public AffineTransform relativeTransform { get; set; }

        /// <summary>
        /// Whether or not this node clip content outside of its bounds.
        /// </summary>
        [DataMember(Name = "clipsContent")]
        public bool clipsContent { get; set; } = false;

        /// <summary>
        /// Whether this layer uses auto-layout to position its children.
        /// </summary>
        [DataMember(Name = "layoutMode")]
        public LayoutMode layoutMode { get; set; }

        /// <summary>
        /// Whether the primary axis has a fixed length (determined by the user) or an automatic length
        /// (determined by the layout engine). This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "primaryAxisSizingMode")]
        public AxisSizingMode primaryAxisSizingMode { get; set; } = AxisSizingMode.Auto;

        /// <summary>
        /// Whether the counter axis has a fixed length (determined by the user) or an automatic length
        /// (determined by the layout engine). This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "counterAxisSizingMode")]
        public AxisSizingMode counterAxisSizingMode { get; set; } = AxisSizingMode.Auto;

        /// <summary>
        /// Determines how the auto-layout frame’s children should be aligned in the primary axis direction.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "primaryAxisAlignItems")]
        public PrimaryAxisAlignItems primaryAxisAlignItems { get; set; } = PrimaryAxisAlignItems.Min;

        /// <summary>
        /// Determines how the auto-layout frame’s children should be aligned in the counter axis direction.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "counterAxisAlignItems")]
        public CounterAxisAlignItems counterAxisAlignItems { get; set; } = CounterAxisAlignItems.Min;

        /// <summary>
        /// The padding between the left border of the frame and its children.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "paddingLeft")]
        public float paddingLeft { get; set; } = 0f;

        /// <summary>
        /// The padding between the right border of the frame and its children.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "paddingRight")]
        public float paddingRight { get; set; } = 0f;

        /// <summary>
        /// The padding between the top border of the frame and its children.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "paddingTop")]
        public float paddingTop { get; set; } = 0f;

        /// <summary>
        /// The padding between the bottom border of the frame and its children.
        /// This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "paddingBottom")]
        public float paddingBottom { get; set; } = 0f;

        /// <summary>
        /// The distance between children of the frame. This property is only applicable for auto-layout frames.
        /// </summary>
        [DataMember(Name = "itemSpacing")]
        public float itemSpacing { get; set; } = 0f;

        /// <summary>
        /// Defines the scrolling behavior of the frame, if there exist contents outside of the frame boundaries.
        /// The frame can either scroll vertically, horizontally, or in both directions to the extents of the content
        /// contained within it. This behavior can be observed in a prototype.
        /// </summary>
        [DataMember(Name = "layoutGrids")]
        public OverflowDirection overflowDirection { get; set; } = OverflowDirection.None;

        /// <summary>
        /// A list of effects attached to this node.
        /// </summary>
        [DataMember(Name = "effects")]
        public List<Effect> effects { get; set; } = new List<Effect>();

        /// <summary>
        /// Does this node mask sibling nodes in front of it?
        /// </summary>
        [DataMember(Name = "isMask")]
        public bool isMask { get; set; } = false;

        /// <summary>
        /// Does this mask ignore fill style (like gradients) and effects?
        /// </summary>
        [DataMember(Name = "isMaskOutline")]
        public bool isMaskOutline { get; set; } = false;
        
        public float[] strokeDashes { get; set; }
        public float strokeMiterAngle { get; set; }
        public StrokeJoin strokeJoin { get; set; }

        public override Node[] GetChildren() => children;
    }

    /// <remarks>
    /// Note that top-level frames (parented directly under the canvas) can still scroll even when
    /// <see cref="OverflowDirection"/> is <see cref="None"/>.
    /// </remarks>
    [DataContract]
    public enum OverflowDirection
    {
        /// <summary>
        ///  The frame does not explicitly scroll.
        /// </summary>
        [EnumMember(Value = "NONE")]
        None,

        /// <summary>
        /// The frame can scroll in the horizontal direction if its content exceeds the frame's bounds horizontally.
        /// </summary>
        [EnumMember(Value = "HORIZONTAL")]
        Horizontal,
        
        /// <summary>
        /// The frame can in the vertical direction if its content exceeds the frame's bounds vertically.
        /// </summary>
        [EnumMember(Value = "VERTICAL")]
        Vertical,
        
        /// <summary>
        /// The frame can scroll in either direction if the content exceeds the frame's bounds.
        /// </summary>
        [EnumMember(Value = "BOTH")]
        Both,
        
        // For backward compability.
        /// <inheritdoc cref="Horizontal"/>
        [EnumMember(Value = "HORIZONTAL_SCROLLING")]
        HorizontalScrolling = Horizontal,

        /// <inheritdoc cref="Vertical"/>
        [EnumMember(Value = "VERTICAL_SCROLLING")]
        VerticalScrolling = Vertical,
        
        /// <inheritdoc cref="Both"/>
        [EnumMember(Value = "HORIZONTAL_AND_VERTICAL_SCROLLING")]
        HorizontalAndVertical = Both
    }

    /// <summary>
    /// Applicable only on auto-layout frames, ignored otherwise. Determines how the auto-layout frame’s children
    /// should be aligned in the primary axis direction.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>In horizontal auto-layout frames, <see cref="Min"/> and <see cref="Max"/> correspond to left and right
    /// respectively.</item>
    /// <item>In vertical auto-layout frames, <see cref="Min"/> and <see cref="Max"/> correspond to top and bottom
    /// respectively.</item>
    /// <item><see cref="SpaceBetween"/> will cause the children to space themselves evenly along the primary axis,
    /// only putting the extra space between the children.</item>
    /// </list>
    ///
    /// The corresponding property for the counter axis direction is <see cref="CounterAxisAlignItems"/>
    /// </remarks>
    [DataContract]
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

    /// <summary>
    /// Applicable only on auto-layout frames, ignored otherwise. Determines how the auto-layout frame’s children
    /// should be aligned in the counter axis direction.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>In horizontal auto-layout frames, <see cref="Min"/> and <see cref="Max"/> correspond to top and bottom
    /// respectively.</item>
    /// <item>In vertical auto-layout frames, <see cref="Min"/> and <see cref="Max"/> correspond to left and right
    /// respectively.</item>
    /// <item><see cref="Baseline"/> can only be set on horizontal auto-layout frames, and aligns all children along the
    /// text baseline.</item>
    /// </list>
    /// The corresponding property for the primary axis direction is <see cref="PrimaryAxisAlignItems"/>
    /// </remarks>
    [DataContract]
    public enum CounterAxisAlignItems
    {
        [EnumMember(Value = "MIN")]
        Min,

        [EnumMember(Value = "CENTER")]
        Center,

        [EnumMember(Value = "MAX")]
        Max,
        
        [EnumMember(Value = "BASELINE")]
        Baseline
    }

    /// <summary>
    /// Applicable only on auto-layout frames. Determines whether the primary or counter axis has a fixed length
    /// (determined by the user) or an automatic length (determined by the layout engine).
    /// </summary>
    [DataContract]
    public enum AxisSizingMode
    {
        [EnumMember(Value = "AUTO")]
        Auto,

        [EnumMember(Value = "FIXED")]
        Fixed
    }

    /// <summary>
    /// Determines whether this layer uses auto-layout to position its children.
    /// </summary>
    [DataContract]
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
    /// Applicable only on direct children of auto-layout frames, ignored otherwise. Determines if the layer should
    /// stretch along the parent’s counter axis.
    /// <list type="bullet">
    /// <item>
    /// In horizontal auto-layout frames, <see cref="Min"/> and <see cref="Max"/> correspond to top and bottom.
    /// </item>
    /// <item>
    /// In vertical auto-layout frames, <see cref="Min"/> and <see cref="Max"/> correspond to left and right.
    /// </item>
    /// </list>
    /// </summary>
    [DataContract]
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

    /// <summary>
    /// The alignment of the stroke with respect to the boundaries of the shape.
    /// </summary>
    [DataContract]
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