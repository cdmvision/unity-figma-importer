using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
        /// Additional assets required for the importer.
        /// </summary>
        public IDictionary<string, Object> graphics { get; set; } = new Dictionary<string, Object>();
        
        /// <summary>
        /// List of fonts required for the importer.
        /// </summary>
        public IDictionary<FontDescriptor, Object> fonts { get; set; } = new Dictionary<FontDescriptor, Object>();
    }
}