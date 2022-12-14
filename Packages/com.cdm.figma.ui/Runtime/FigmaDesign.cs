using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaDesign : Figma.FigmaDesign
    {
        [SerializeField]
        private FigmaDocument _document;

        /// <summary>
        /// Get document which is root node of the Figma file.
        /// </summary>
        public FigmaDocument document => _document;
        
        [SerializeField]
        private List<Binding> _bindings = new List<Binding>();

        /// <summary>
        /// Gets all bindings in the whole Figma design document.
        /// </summary>
        public IReadOnlyList<Binding> bindings => _bindings;

        public static T Create<T>(FigmaFile file, FigmaDocument document, IEnumerable<Binding> bindings) 
            where T : FigmaDesign
        {
            var figmaDesign = Create<T>(file);
            figmaDesign._document = document;
            figmaDesign._bindings.AddRange(bindings);
            
            document.TraverseDfs(node =>
            {
                node.figmaDesign = figmaDesign;
                return true;
            });
            
            return figmaDesign;
        }
    }
}