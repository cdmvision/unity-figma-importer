using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Guides to align and place objects within a frame.
    /// 
    /// An array of layout grids attached to this node (see layout grids section
    /// for more details). GROUP nodes do not have this attribute
    /// </summary>
    [DataContract]
    public class LayoutGrid
    {
        /// <summary>
        /// Positioning of grid as a string enum
        /// "MIN": Grid starts at the left or top of the frame
        /// "MAX": Grid starts at the right or bottom of the frame
        /// "CENTER": Grid is center aligned
        /// </summary>
        [DataMember(Name = "alignment")]
        public Alignment alignment { get; set; }

        /// <summary>
        /// Color of the grid
        /// </summary>
        [DataMember(Name = "color")]
        public Color color { get; set; }

        /// <summary>
        /// Number of columns or rows
        /// </summary>
        [DataMember(Name = "count")]
        public float count { get; set; }

        /// <summary>
        /// Spacing in between columns and rows
        /// </summary>
        [DataMember(Name = "gutterSize")]
        public float gutterSize { get; set; }

        /// <summary>
        /// Spacing before the first column or row
        /// </summary>
        [DataMember(Name = "offset")]
        public float offset { get; set; }

        /// <summary>
        /// Orientation of the grid as a string enum
        /// "COLUMNS": Vertical grid
        /// "ROWS": Horizontal grid
        /// "GRID": Square grid
        /// </summary>
        [DataMember(Name = "pattern")]
        public Pattern pattern { get; set; }

        /// <summary>
        /// Width of column grid or height of row grid or square grid spacing
        /// </summary>
        [DataMember(Name = "sectionSize")]
        public float sectionSize { get; set; }

        /// <summary>
        /// Is the grid currently visible?
        /// </summary>
        [DataMember(Name = "visible")]
        public bool visible { get; set; } = true;
    }
    
    /// <summary>
    /// Positioning of the grid.
    /// </summary>
    [DataContract]
    public enum Alignment
    {
        /// <summary>
        /// Grid is center aligned.
        /// </summary>
        [EnumMember(Value = "CENTER")]
        Center,

        /// <summary>
        /// Grid starts at the right or bottom of the frame.
        /// </summary>
        [EnumMember(Value = "MAX")]
        Max,

        /// <summary>
        /// Grid starts at the left or top of the frame.
        /// </summary>
        [EnumMember(Value = "MIN")]
        Min,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "STRETCH")]
        Stretch
    }
    
    /// <summary>
    /// Orientation of the grid.
    /// </summary>
    [DataContract]
    public enum Pattern
    {
        /// <summary>
        /// Vertical grid.
        /// </summary>
        [EnumMember(Value = "COLUMNS")]
        Columns,

        /// <summary>
        /// Square grid.
        /// </summary>
        [EnumMember(Value = "GRID")]
        Grid,

        /// <summary>
        /// Horizontal grid.
        /// </summary>
        [EnumMember(Value = "ROWS")]
        Rows
    }
}