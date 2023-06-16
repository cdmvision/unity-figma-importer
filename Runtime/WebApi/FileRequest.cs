namespace Cdm.Figma
{
    public class FileRequest : BaseRequest
    {
        /// <summary>
        /// The document referred to by :key as a JSON object. The file key can be parsed from any
        /// Figma file url: <c>https://www.figma.com/file/:key/:title</c>
        /// </summary>
        public string fileId { get; }
        
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
        public string geometry { get; set; }
        
        /// <summary>
        /// Array of plugin IDs. Any data present in the document written by those plugins will be included in the
        /// result in the <see cref="FigmaFile.pluginData"/> and `<see cref="FigmaFile.sharedPluginData"/> properties.
        /// </summary>
        public string[] plugins { get; set; }

        public FileRequest(string fileId)
        {
            this.fileId = fileId;
        }
    }
}