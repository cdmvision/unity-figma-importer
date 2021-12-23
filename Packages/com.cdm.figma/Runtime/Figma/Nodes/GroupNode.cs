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
    public class GroupNode : SceneNode, INodeTransform, INodeLayout, INodeBlend, INodeTransition, INodeExport
    {
        public override string type => NodeType.Group;
        
        /// <summary>
        /// A list of nodes that are direct children of this node.
        /// </summary>
        [DataMember(Name = "children")]
        public Node[] children { get; set; }
        
        /// <summary>
        /// An array of fill paints applied to the node.
        /// </summary>
        [DataMember(Name = "fills")]
        public List<Paint> fills { get; private set; } = new List<Paint>();

        /// <summary>
        /// An array of stroke paints applied to the node.
        /// </summary>
        [DataMember(Name = "strokes")]
        public List<Paint> strokes { get; private set; } = new List<Paint>();

        /// <summary>
        /// The weight of strokes on the node.
        /// </summary>
        [DataMember(Name = "strokeWeight")]
        public float? strokeWeight { get; set; }

        /// <summary>
        /// Position of stroke relative to vector outline.
        /// </summary>
        [DataMember(Name = "strokeAlign")]
        public StrokeAlign? strokeAlign { get; set; }

        /// <summary>
        /// Radius of each corner of the frame if a single radius is set for all corners.
        /// </summary>
        [DataMember(Name = "cornerRadius")]
        public float? cornerRadius { get; set; }

        /// <summary>
        /// Array of length 4 of the radius of each corner of the frame, starting in the top left and
        /// proceeding clockwise.
        /// </summary>
        [DataMember(Name = "rectangleCornerRadii")]
        public float[] rectangleCornerRadii { get; set; }

        /// <summary>
        /// A list of export settings representing images to export from the canvas.
        /// </summary>
        [DataMember(Name = "exportSettings")]
        public List<ExportSetting> exportSettings { get; set; } = new List<ExportSetting>();

        /// <summary>
        /// How this node blends with nodes behind it in the scene.
        /// </summary>
        [DataMember(Name = "blendMode")]
        public BlendMode? blendMode { get; set; }

        /// <summary>
        /// Keep height and width constrained to same ratio.
        /// </summary>
        [DataMember(Name = "preserveRatio")]
        public bool preserveRatio { get; set; } = false;

        /// <summary>
        /// Group node does not have this property.
        /// </summary>
        public int? layoutGrow { get; set; } = null;

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
        /// Opacity of the node.
        /// </summary>
        [DataMember(Name = "opacity")]
        public float opacity { get; set; } = 1f;

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
        
        public override Node[] GetChildren() => children;
    }

    [DataContract]
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

    [DataContract]
    public enum CounterAxisAlignItems
    {
        [EnumMember(Value = "MIN")]
        Min,

        [EnumMember(Value = "CENTER")]
        Center,

        [EnumMember(Value = "MAX")]
        Max
    }

    [DataContract]
    public enum AxisSizingMode
    {
        [EnumMember(Value = "AUTO")]
        Auto,

        [EnumMember(Value = "FIXED")]
        Fixed
    }

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
    /// In horizontal auto-layout frames, "MIN" and "MAX" correspond to "TOP" and "BOTTOM".
    /// In vertical auto-layout frames, "MIN" and "MAX" correspond to "LEFT" and "RIGHT".
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