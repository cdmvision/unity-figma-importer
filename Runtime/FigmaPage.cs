using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [SelectionBase]
    public class FigmaPage : FigmaNode
    {
        [SerializeField]
        private List<FigmaImporterLogReference> _allLogs = new List<FigmaImporterLogReference>();
        
        /// <summary>
        /// All logs containing the children's as well as itself.
        /// </summary>
        public List<FigmaImporterLogReference> allLogs => _allLogs;
    }
}