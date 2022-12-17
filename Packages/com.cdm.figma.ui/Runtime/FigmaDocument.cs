using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{    
    [SelectionBase]
    public class FigmaDocument : FigmaNode
    {
        [SerializeField]
        private List<FigmaImporterLogReference> _allLogs = new List<FigmaImporterLogReference>();
        
        public List<FigmaImporterLogReference> allLogs => _allLogs;

        [SerializeField]
        private List<FigmaPage> _pages = new List<FigmaPage>();

        public IReadOnlyList<FigmaPage> pages => _pages;
        
        internal void InitPages()
        {
            _pages.Clear();
            foreach (Transform child in transform)
            {
                var page = child.GetComponent<FigmaPage>();
                if (page != null)
                {
                    _pages.Add(page);
                }
            }
        }
    }
}