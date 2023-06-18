using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The vector node is our most general representation of shape, allowing you to specify individual vertices,
    /// segments, and regions. They are typically created in the UI using the pen tool, pencil tool, or when
    /// flattening a selection.
    ///
    /// Note that similarly to groups, <see cref="VectorNode"/>s' position and size are automatically adjusted to
    /// fit its vertices. For example, the positions returned by vector paths is relative to the vector object position.
    /// </summary>
    [DataContract]
    public class VectorNode : SceneNode, INodeTransform, INodeLayout, INodeFill, INodeTransition, INodeExport
    {
        public override string type => NodeType.Vector;
        
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
        /// Determines if the layer should stretch along the parent’s counter axis. This property is only provided for
        /// direct children of auto-layout frames.
        /// </summary>
        [DataMember(Name = "layoutAlign")]
        public LayoutAlign layoutAlign { get; set; }

        /// <summary>
        /// This property is applicable only for direct children of auto-layout frames, ignored otherwise.
        /// Determines whether a layer should stretch along the parent’s primary axis. A 0 corresponds to a fixed size
        /// and 1 corresponds to stretch.
        /// </summary>
        [DataMember(Name = "layoutGrow")]
        public int? layoutGrow { get; set; }
        
        /// <summary>
        /// Horizontal and vertical layout constraints for node.
        /// </summary>
        [DataMember(Name = "constraints")]
        public LayoutConstraint constraints { get; set; }
        
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
        /// A list of effects attached to this node.
        /// </summary>
        [DataMember(Name = "effects")]
        public List<Effect> effects { get; set; } = new List<Effect>();
        
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
        /// Does this node mask sibling nodes in front of it?
        /// </summary>
        [DataMember(Name = "isMask")]
        public bool isMask { get; set; } = false;

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
        /// Only specified if parameter geometry=paths is used. An array of paths representing the object fill.
        /// </summary>
        [DataMember(Name = "fillGeometry")]
        public VectorPath[] fillGeometry { get; set; }

        /// <summary>
        /// The weight of strokes on the node.
        /// </summary>
        [DataMember(Name = "strokeWeight")]
        public float? strokeWeight { get; set; }

        /// <summary>
        /// The stroke cap type.
        /// </summary>
        [DataMember(Name = "strokeCap")]
        public StrokeCap strokeCap { get; set; } = StrokeCap.None;
        
        /// <summary>
        /// The stroke join type.
        /// </summary>
        [DataMember(Name = "strokeJoin")]
        public StrokeJoin strokeJoin { get; set; } = StrokeJoin.Miter;
        
        /// <summary>
        /// An array of floating point numbers describing the pattern of dash length and gap lengths that the vector
        /// path follows. For example a value of [1, 2] indicates that the path has a dash of length 1 followed
        /// by a gap of length 2, repeated.
        /// </summary>
        [DataMember(Name = "strokeDashes")]
        public float[] strokeDashes { get; set; }

        /// <summary>
        /// Only valid if <see cref="strokeJoin"/> is <see cref="StrokeJoin.Miter"/>. The corner angle, in degrees,
        /// below which <see cref="strokeJoin"/>  will be set to <see cref="StrokeJoin.Bevel"/> to avoid super
        /// sharp corners. By default this is 28.96 degrees.
        /// </summary>
        [DataMember(Name = "strokeMiterAngle")]
        public float strokeMiterAngle { get; set; } = 28.96f;
        
        /// <summary>
        /// Only specified if parameter geometry=paths is used. An array of paths representing the object stroke.
        /// </summary>
        [DataMember(Name = "strokeGeometry")]
        public VectorPath[] strokeGeometry { get; set; }
        
        /// <summary>
        /// Position of stroke relative to vector outline.
        /// </summary>
        [DataMember(Name = "strokeAlign")]
        public StrokeAlign? strokeAlign { get; set; }

        /// <summary>
        /// A mapping of a <see cref="StyleType"/>> to style ID (see <see cref="Style"/>) of styles present on this node. The style ID can be used
        /// to look up more information about the style in the top-level styles field.
        /// </summary>
        [DataMember(Name = "styles")]
        public Dictionary<StyleType, string> styles { get; private set; } = new Dictionary<StyleType, string>();

        /// <summary>
        /// Map from ID to <see cref="PaintOverride"/> for looking up fill overrides. To see which regions are
        /// overriden, you must use the <see cref="FileRequest.geometry"/> = paths option. Each path returned may
        /// have an <see cref="VectorPath.overrideId"/> which maps to this table.
        /// </summary>
        [DataMember(Name = "fillOverrideTable")]
        public Dictionary<int, PaintOverride> fillOverrideTable { get; private set; } =
            new Dictionary<int, PaintOverride>();

        [DataContract]
        public enum StyleType
        {
            [EnumMember(Value = "FILL")]
            Fill,
            
            [EnumMember(Value = "STROKE")]
            Stroke,
            
            [EnumMember(Value = "GRID")]
            Grid,
            
            [EnumMember(Value = "TEXT")]
            Text,
            
            [EnumMember(Value = "EFFECT")]
            Effect,
        }
    }

    [DataContract]
    public enum StrokeCap
    {
        [EnumMember(Value = "NONE")]
        None,
        
        [EnumMember(Value = "ROUND")]
        Round,
        
        [EnumMember(Value = "SQUARE")]
        Square,
        
        [EnumMember(Value = "LINE_ARROW")]
        LineArrow,
        
        [EnumMember(Value = "TRIANGLE_ARROW")]
        TriangleArrow
    }
    
    [DataContract]
    public enum StrokeJoin
    {
        [EnumMember(Value = "MITER")]
        Miter,
        
        [EnumMember(Value = "BEVEL")]
        Bevel,
        
        [EnumMember(Value = "ROUND")]
        Round
    }
}