using System;

namespace Cdm.Figma
{
    public interface IFigmaImporter
    {
        /// <summary>
        /// Imports pages and all their nodes from Figma file given into Unity.
        /// </summary>
        /// <param name="file">The Figma file to be imported.</param>
        /// <param name="options">Importer options.</param>
        FigmaDesign ImportFile(FigmaFile file, Options options = null);

        public class Options
        {
            public string[] selectedPages { get; set; }

            /// <summary>
            /// Called as each page is about to be converted, with the page name and how far through
            /// the selected pages this is, from 0 to 1.
            /// </summary>
            /// <remarks>
            /// A plain callback, so this stays independent of the editor.
            /// </remarks>
            public Action<string, float> onPageProgress { get; set; }
        }
    }
}