using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaDesign : Figma.FigmaDesign
    {
        [SerializeField]
        private List<FigmaPageNode> _pages = new List<FigmaPageNode>();

        /// <summary>
        /// Gets all imported Figma pages.
        /// </summary>
        public IReadOnlyList<FigmaPageNode> pages => _pages;

        public static T Create<T>(FigmaFile file, IEnumerable<FigmaPageNode> pages) where T : FigmaDesign
        {
            var figmaDesign = Create<T>(file);
            figmaDesign._pages.AddRange(pages);
            return figmaDesign;
        }
    }
}