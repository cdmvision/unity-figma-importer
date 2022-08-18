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

        public static T Create<T>(FigmaFile file, IEnumerable<FigmaPage> pages) where T : FigmaDesign
        {
            var figmaDesign = Create<T>(file);
            figmaDesign._pages.AddRange(pages);
            return figmaDesign;
        }
    }
}