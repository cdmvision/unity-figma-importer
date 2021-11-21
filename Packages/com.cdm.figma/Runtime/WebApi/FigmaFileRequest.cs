namespace Cdm.Figma
{
    public class FigmaFileRequest : FigmaBaseRequest
    {
        /// <summary>
        /// A specific version ID to get. Omitting this will get the current version of the file.
        /// </summary>
        public string version { get; set; }
        
        /// <summary>
        /// Positive integer representing how deep into the document tree to traverse. For example, setting this to 1
        /// returns only Pages, setting it to 2 returns Pages and all top level objects on each page. Not setting
        /// this parameter returns all nodes.
        /// </summary>
        public int? depth { get; set; }
        
        /// <summary>
        /// Set to "paths" to export vector data.
        /// </summary>
        public int? geometry { get; set; }

        public FigmaFileRequest(string personalAccessToken, string fileId)
            : base(personalAccessToken, fileId)
        {
        }
    }
}