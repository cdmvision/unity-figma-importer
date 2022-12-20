namespace Cdm.Figma
{
    public class ImageFillsRequest : BaseRequest
    {
        /// <summary>
        /// The document referred to by :key as a JSON object. The file key can be parsed from any
        /// Figma file url: <c>https://www.figma.com/file/:key/:title</c>
        /// </summary>
        public string fileId { get; }
        
        /// <summary>
        /// The image references to be downloaded. Leave as empty to download all the images in the file.
        /// </summary>
        /// <seealso cref="ImagePaint.imageRef"/>
        public string[] imageRefs { get; set; }
        
        public ImageFillsRequest(string fileId)
        {
            this.fileId = fileId;
        }
    }
}