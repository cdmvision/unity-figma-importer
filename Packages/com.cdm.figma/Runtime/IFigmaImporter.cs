using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cdm.Figma
{
    public interface IFigmaImporter
    {
        /// <summary>
        /// Imports pages and all their nodes from Figma file given into Unity.
        /// </summary>
        /// <param name="file">Figma file to be imported.</param>
        /// <param name="options">Optional import options.</param>
        Task ImportFileAsync(FigmaFile file, FigmaImportOptions options = null);
    }
    
    public class FigmaImportOptions
    {
        /// <summary>
        /// Set page IDs to import only selected pages. Leave as <c>null</c> to import all pages.
        /// </summary>
        public string[] pages;

        /// <summary>
        /// Contains node id as key and asset path as value. Some nodes are imported as an asset
        /// (i.e. <see cref="VectorNode"/> as <see cref="UnityEngine.UIElements.VectorImage"/>).
        ///
        /// For example, you can use an SVG graphic like url("project:///Assets/path_to_asset/graphic_file.svg"); in
        /// the style definition.
        /// </summary>
        public IDictionary<string, string> assets { get; set; } = new Dictionary<string, string>();
    }
}