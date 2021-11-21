using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cdm.Figma
{
    /// <summary>
    /// A solid color, gradient, or image texture that can be applied as fills or strokes.
    /// </summary>
    [Serializable]
    public partial class Paint
    {
        /// <summary>
        /// Type of paint as a string enum
        /// </summary>
        [JsonProperty("type")]
        public PaintType type { get; set; }
        
        /// <summary>
        /// Is the paint enabled?
        /// </summary>
        [JsonProperty("visible", DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool visible { get; set; } = true;

        /// <summary>
        /// Overall opacity of paint (colors within the paint can also have opacity
        /// values which would blend with this)
        /// </summary>
        [JsonProperty("opacity")]
        public float opacity { get; set; } = 1f;

        #region For solid paints

        /// <summary>
        /// Solid color of the paint.
        /// </summary>
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public Color color { get; set; }

        #endregion
        
        #region For gradient paints

        /// <summary>
        /// For gradient paints; how this node blends with nodes behind it in the scene.
        /// </summary>
        /// <seealso cref="BlendMode"/>
        [JsonProperty("blendMode", NullValueHandling = NullValueHandling.Ignore)]
        public BlendMode blendMode { get; set; }

        /// <summary>
        /// This field contains three vectors, each of which are a position in
        /// normalized object space (normalized object space is if the top left
        /// corner of the bounding box of the object is (0, 0) and the bottom
        /// right is (1,1)). The first position corresponds to the start of the
        /// gradient (value 0 for the purposes of calculating gradient stops),
        /// the second position is the end of the gradient (value 1), and the
        /// third handle position determines the width of the gradient (only
        /// relevant for non-linear gradients).
        /// </summary>
        [JsonProperty("gradientHandlePositions", NullValueHandling = NullValueHandling.Ignore)]
        public List<Vector> gradientHandlePositions { get; private set; } = new List<Vector>();

        /// <summary>
        /// Positions of key points along the gradient axis with the colors
        /// anchored there. Colors along the gradient are interpolated smoothly
        /// between neighboring gradient stops.
        /// </summary>
        [JsonProperty("gradientStops", NullValueHandling = NullValueHandling.Ignore)]
        public List<ColorStop> gradientStops { get; private set; } = new List<ColorStop>();

        #endregion

        #region For image paints
        
        /// <summary>
        /// Image scaling mode.
        /// </summary>
        [JsonProperty("scaleMode", NullValueHandling = NullValueHandling.Ignore)]
        public ScaleMode scaleMode { get; set; }
        
        /// <summary>
        /// Affine transform applied to the image, only present if <see cref="scaleMode"/> is
        /// <see cref="ScaleMode.Stretch"/>
        /// </summary>
        [JsonProperty("imageTransform", NullValueHandling = NullValueHandling.Ignore)]
        public AffineTransform imageTransform { get; set; }
        
        /// <summary>
        /// Amount image is scaled by in tiling, only present if <see cref="scaleMode"/> is
        /// <see cref="ScaleMode.Tile"/>.
        /// </summary>
        [JsonProperty("scalingFactor")]
        public float scalingFactor { get; set; }
        
        /// <summary>
        /// Image rotation, in degrees.
        /// </summary>
        [JsonProperty("rotation")]
        public float rotation { get; set; }
        
        /// <summary>
        /// A reference to an image embedded in this node.
        /// </summary>
        [JsonProperty("imageRef")]
        public string imageRef { get; set; }

        /// <summary>
        /// Defines what image filters have been applied to this paint, if any. If this property is not defined,
        /// no filters have been applied.
        /// </summary>
        [JsonProperty("filters")]
        public ImageFilters? filters { get; set; }

        /// <summary>
        /// A reference to the GIF embedded in this node, if the image is a GIF.
        /// </summary>
        [JsonProperty("gifRef")]
        public string gifRef { get; set; }
        
        #endregion
    }
    
    [Serializable]
    public enum PaintType
    {
        [EnumMember(Value = "SOLID")]
        Solid,

        [EnumMember(Value = "GRADIENT_LINEAR")]
        GradientLinear,

        [EnumMember(Value = "GRADIENT_RADIAL")]
        GradientRadial,

        [EnumMember(Value = "GRADIENT_ANGULAR")]
        GradientAngular,

        [EnumMember(Value = "GRADIENT_DIAMOND")]
        GradientDiamond,

        [EnumMember(Value = "IMAGE")]
        Image,

        [EnumMember(Value = "EMOJI")]
        Emoji,
    };
    
    /// <summary>
    /// A position color pair representing a gradient stop.
    ///
    /// Positions of key points along the gradient axis with the colors
    /// anchored there. Colors along the gradient are interpolated smoothly
    /// between neighboring gradient stops.
    /// </summary>
    [Serializable]
    public class ColorStop
    {
        /// <summary>
        /// Value between 0 and 1 representing position along gradient axis.
        /// </summary>
        [JsonProperty("position")]
        public float position { get; set; }
        
        /// <summary>
        /// Color attached to corresponding position.
        /// </summary>
        [JsonProperty("color")]
        public Color color { get; set; }
    }
}