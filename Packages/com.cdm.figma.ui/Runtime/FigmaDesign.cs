using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaDesign : Figma.FigmaDesign
    {
        [SerializeField]
        private List<PageNodeObject> _pages = new List<PageNodeObject>();

        /// <summary>
        /// Gets all imported Figma pages.
        /// </summary>
        public IReadOnlyList<PageNodeObject> pages => _pages;

        public static T Create<T>(FigmaFile file, IEnumerable<PageNodeObject> pages) where T : FigmaDesign
        {
            var figmaDesign = Create<T>(file);
            figmaDesign._pages.AddRange(pages);
            return figmaDesign;
        }
    }
}