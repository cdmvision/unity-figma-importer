namespace Cdm.Figma
{
    public class ImageRequest : BaseRequest
    {
        /// <summary>
        /// The document referred to by :key as a JSON object. The file key can be parsed from any
        /// Figma file url: <c>https://www.figma.com/file/:key/:title</c>
        /// </summary>
        public string fileId { get; }
        
        /// <summary>
        /// List of node IDs to render.
        /// </summary>
        public string[] ids { get; set; }
        
        /// <summary>
        /// A number between 0.01 and 4, the image scaling factor.
        /// </summary>
        public float? scale { get; set; }
        
        /// <summary>
        /// Image output format, can be jpg, png, svg, or pdf.
        /// </summary>
        public string format { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string version { get; set; }
        
        /// <summary>
        /// Whether to include id attributes for all SVG elements. Default: false.
        /// </summary>
        public bool svgIncludeId { get; set; } = false;
        
        /// <summary>
        /// Whether to simplify inside/outside strokes and use stroke attribute if possible instead of <mask>.
        /// Default: true.
        /// </summary>
        public bool svgSimplifyStroke { get; set; } = true;
        
        /// <summary>
        /// Use the full dimensions of the node regardless of whether or not it is cropped or the space around it is
        /// empty. Use this to export text nodes without cropping. Default: false.
        /// </summary>
        public bool useAbsoluteBounds { get; set; } = false;
        
        public ImageRequest(string fileId)
        {
            this.fileId = fileId;
        }
    }
}