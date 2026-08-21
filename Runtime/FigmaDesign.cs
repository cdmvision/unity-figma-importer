using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaDesign : Figma.FigmaDesign
    {
        /// <summary>
        /// Get document which is root node of the Figma file.
        /// </summary>
        public FigmaDocument document => GetComponent<FigmaDocument>();
        
        [SerializeField]
        private List<Binding> _bindings = new List<Binding>();

        /// <summary>
        /// Gets all bindings in the whole Figma design document.
        /// </summary>
        public IReadOnlyList<Binding> bindings => _bindings;

        internal void SetBindings(List<Binding> bindingList)
        {
            _bindings = bindingList;
        }
    }
}