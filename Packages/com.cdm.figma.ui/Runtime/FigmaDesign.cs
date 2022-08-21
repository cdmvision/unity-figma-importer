using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaDesign : Figma.FigmaDesign
    {
        [SerializeField]
        private List<FigmaPage> _pages = new List<FigmaPage>();

        /// <summary>
        /// Gets all imported Figma pages.
        /// </summary>
        public IReadOnlyList<FigmaPage> pages => _pages;
        
        [SerializeField]
        private List<Binding> _bindings = new List<Binding>();

        /// <summary>
        /// Gets all bindings in the whole Figma design document.
        /// </summary>
        public IReadOnlyList<Binding> bindings => _bindings;

        public static T Create<T>(FigmaFile file, IEnumerable<FigmaPage> pages, IEnumerable<Binding> bindings) 
            where T : FigmaDesign
        {
            var figmaDesign = Create<T>(file);
            figmaDesign._pages.AddRange(pages);
            figmaDesign._bindings.AddRange(bindings);
            return figmaDesign;
        }
    }
}