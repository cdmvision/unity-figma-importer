using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [SelectionBase]
    public class FigmaPage : FigmaNode
    {
        [SerializeField]
        private List<FigmaImporterLogReference> _allLogs = new List<FigmaImporterLogReference>();
        
        public List<FigmaImporterLogReference> allLogs => _allLogs;
    }
}