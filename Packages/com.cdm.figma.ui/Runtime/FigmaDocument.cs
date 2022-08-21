using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{    
    [SelectionBase]
    public class FigmaDocument : FigmaNode, IEnumerable<FigmaPage>
    {
        [SerializeField]
        private List<FigmaImporterLogReference> _allLogs = new List<FigmaImporterLogReference>();
        
        public List<FigmaImporterLogReference> allLogs => _allLogs;

        public IEnumerator<FigmaPage> GetPages()
        {
            return GetEnumerator();
        }
        
        public IEnumerator<FigmaPage> GetEnumerator()
        {
            foreach (Transform child in transform)
            {
                var figmaPage = child.GetComponent<FigmaPage>();
                if (figmaPage != null)
                {
                    yield return figmaPage;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}