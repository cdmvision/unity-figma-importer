using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A solid color, gradient, or image texture that can be applied as fills or strokes.
    /// </summary>
    [DataContract]
    public class Paint
    {
        /// <summary>
        /// Type of paint as a string enum
        /// </summary>
        [DataMember(Name = "type")]
        public PaintType type { get; set; }
        
        /// <summary>
        /// Is the paint enabled?
        /// </summary>
        [DataMember(Name = "visible")]
        public bool visible { get; set; } = true;

        /// <summary>
        /// Overall opacity of paint (colors within the paint can also have opacity
        /// values which would blend with this)
        /// </summary>
        [DataMember(Name = "opacity")]
        public float opacity { get; set; } = 1f;

        #region For solid paints

        /// <summary>
        /// Solid color of the paint.
        /// </summary>
        [DataMember(Name = "color")]
        public Color color { get; set; }

        #endregion
        
        #region For gradient paints

        /// <summary>
        /// For gradient paints; how this node blends with nodes behind it in the scene.
        /// </summary>
        /// <seealso cref="BlendMode"/>
        [DataMember(Name = "blendMode")]
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
        [DataMember(Name = "gradientHandlePositions")]
        public List<Vector> gradientHandlePositions { get; private set; } = new List<Vector>();

        /// <summary>
        /// Positions of key points along the gradient axis with the colors
        /// anchored there. Colors along the gradient are interpolated smoothly
        /// between neighboring gradient stops.
        /// </summary>
        [DataMember(Name = "gradientStops")]
        public List<ColorStop> gradientStops { get; private set; } = new List<ColorStop>();

        #endregion

        #region For image paints
        
        /// <summary>
        /// Image scaling mode.
        /// </summary>
        [DataMember(Name = "scaleMode")]
        public ScaleMode scaleMode { get; set; }
        
        /// <summary>
        /// Affine transform applied to the image, only present if <see cref="scaleMode"/> is
        /// <see cref="ScaleMode.Stretch"/>
        /// </summary>
        [DataMember(Name = "imageTransform")]
        public AffineTransform imageTransform { get; set; }
        
        /// <summary>
        /// Amount image is scaled by in tiling, only present if <see cref="scaleMode"/> is
        /// <see cref="ScaleMode.Tile"/>.
        /// </summary>
        [DataMember(Name = "scalingFactor")]
        public float scalingFactor { get; set; }
        
        /// <summary>
        /// Image rotation, in degrees.
        /// </summary>
        [DataMember(Name = "rotation")]
        public float rotation { get; set; }
        
        /// <summary>
        /// A reference to an image embedded in this node.
        /// </summary>
        [DataMember(Name = "imageRef")]
        public string imageRef { get; set; }

        /// <summary>
        /// Defines what image filters have been applied to this paint, if any. If this property is not defined,
        /// no filters have been applied.
        /// </summary>
        [DataMember(Name = "filters")]
        public ImageFilters filters { get; set; }

        /// <summary>
        /// A reference to the GIF embedded in this node, if the image is a GIF.
        /// </summary>
        [DataMember(Name = "gifRef")]
        public string gifRef { get; set; }
        
        #endregion
    }
    
    [DataContract]
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
    }
    
    [DataContract]
    public enum ScaleMode
    {
        [EnumMember(Value = "FILL")]
        Fill,
        
        [EnumMember(Value = "FIT")]
        Fit,
        
        [EnumMember(Value = "TILE")]
        Tile,
        
        [EnumMember(Value = "STRETCH")]
        Stretch
    }
    
    /// <summary>
    /// Defines the image filters applied to an image paint. All values are from -1 to 1.
    /// </summary>
    [DataContract]
    public class ImageFilters
    {
        [DataMember(Name = "exposure")]
        public float exposure { get; set; }

        [DataMember(Name = "contrast")]
        public float contrast { get; set; }

        [DataMember(Name = "saturation")]
        public float saturation { get; set; }

        [DataMember(Name = "temperature")]
        public float temperature { get; set; }

        [DataMember(Name = "tint")]
        public float tint { get; set; }

        [DataMember(Name = "highlights")]
        public float highlights { get; set; }

        [DataMember(Name = "shadows")]
        public float shadows { get; set; }
    }
    
    /// <summary>
    /// A position color pair representing a gradient stop.
    ///
    /// Positions of key points along the gradient axis with the colors
    /// anchored there. Colors along the gradient are interpolated smoothly
    /// between neighboring gradient stops.
    /// </summary>
    [DataContract]
    public class ColorStop
    {
        /// <summary>
        /// Value between 0 and 1 representing position along gradient axis.
        /// </summary>
        [DataMember(Name = "position")]
        public float position { get; set; }
        
        /// <summary>
        /// Color attached to corresponding position.
        /// </summary>
        [DataMember(Name = "color")]
        public Color color { get; set; }
    }
}