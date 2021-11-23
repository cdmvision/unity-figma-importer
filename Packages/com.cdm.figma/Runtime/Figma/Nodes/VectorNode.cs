using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class VectorNode : Node
    {
        public override NodeType type => NodeType.Vector;
        
        /// <summary>
        /// If true, layer is locked and cannot be edited.
        /// </summary>
        [JsonProperty("locked")]
        public bool locked { get; private set; } = false;
        
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
        /// Determines if the layer should stretch along the parent’s counter axis. This property is only provided for
        /// direct children of auto-layout frames.
        /// </summary>
        [JsonProperty("layoutAlign")]
        public LayoutAlign layoutAlign { get; set; }

        /// <summary>
        /// This property is applicable only for direct children of auto-layout frames, ignored otherwise.
        /// Determines whether a layer should stretch along the parent’s primary axis. A 0 corresponds to a fixed size
        /// and 1 corresponds to stretch.
        /// </summary>
        [JsonProperty("layoutGrow")]
        public int layoutGrow { get; set; } = 0;
        
        /// <summary>
        /// Horizontal and vertical layout constraints for node.
        /// </summary>
        [JsonProperty("constraints")]
        public LayoutConstraint constraints { get; set; }
        
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
        /// A list of effects attached to this node.
        /// </summary>
        [JsonProperty("effects")]
        public List<Effect> effects { get; private set; } = new List<Effect>();
        
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
        /// Does this node mask sibling nodes in front of it?
        /// </summary>
        [JsonProperty("isMask")]
        public bool isMask { get; set; } = false;
        
        /// <summary>
        /// An array of fill paints applied to the node.
        /// </summary>
        [JsonProperty("fills")]
        public List<Paint> fills { get; private set; } = new List<Paint>();
        
        // TODO:
        /// <summary>
        /// Only specified if parameter geometry=paths is used. An array of paths representing the object fill.
        /// </summary>
        //[JsonProperty("fillGeometry")]
        //public List<Path> fillGeometry { get; private set; } = new List<Path>();
        
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
        /// The stroke cap type.
        /// </summary>
        [JsonProperty("strokeCap")]
        public StrokeCap strokeCap { get; set; } = StrokeCap.None;
        
        /// <summary>
        /// The stroke join type.
        /// </summary>
        [JsonProperty("strokeCap")]
        public StrokeJoin strokeJoin { get; set; } = StrokeJoin.Miter;
        
        /// <summary>
        /// An array of floating point numbers describing the pattern of dash length and gap lengths that the vector
        /// path follows. For example a value of [1, 2] indicates that the path has a dash of length 1 followed
        /// by a gap of length 2, repeated.
        /// </summary>
        [JsonProperty("strokeDashes")]
        public float[] strokeDashes { get; set; }

        /// <summary>
        /// Only valid if <see cref="strokeJoin"/> is <see cref="StrokeJoin.Miter"/>. The corner angle, in degrees,
        /// below which <see cref="strokeJoin"/>  will be set to <see cref="StrokeJoin.Bevel"/> to avoid super
        /// sharp corners. By default this is 28.96 degrees.
        /// </summary>
        [JsonProperty("strokeMiterAngle")]
        public float strokeMiterAngle { get; set; } = 28.96f;
        
        // TODO:
        /// <summary>
        /// Only specified if parameter geometry=paths is used. An array of paths representing the object stroke.
        /// </summary>
        //[JsonProperty("strokeGeometry")]
        //public List<Path> strokeGeometry { get; private set; } = new List<Path>();
        
        /// <summary>
        /// Position of stroke relative to vector outline.
        /// </summary>
        [JsonProperty("strokeAlign")]
        public StrokeAlign? strokeAlign { get; set; }

        /// <summary>
        /// A mapping of a <see cref="StyleType"/>> to style ID (see <see cref="Style"/>) of styles present on this node. The style ID can be used
        /// to look up more information about the style in the top-level styles field.
        /// </summary>
        [JsonProperty("styles")]
        public Dictionary<StyleType, string> styles { get; private set; } = new Dictionary<StyleType, string>();
    }

    [Serializable]
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
    
    [Serializable]
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