using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ImagePaint : Paint
    {
        public override string type => PaintType.Image;
        
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
        public float? scalingFactor { get; set; }

        /// <summary>
        /// Image rotation, in degrees.
        /// </summary>
        [DataMember(Name = "rotation")]
        public float rotation { get; set; } = 0;
        
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

        public bool isGif => !string.IsNullOrEmpty(gifRef);
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
}