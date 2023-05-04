using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cdm.Figma.UI
{    
    [SelectionBase]
    public class FigmaDocument : FigmaNode
    {
        [SerializeField]
        private List<FigmaPage> _pages = new List<FigmaPage>();

        public IList<FigmaPage> pages => _pages;
        
        public static FigmaDocument Instantiate(FigmaDocument original, Transform parent = null)
        {
            var document = Object.Instantiate(original, parent);
            
            document._pages.Clear();
            foreach (var page in original.pages)
            {
                document._pages.Add(Object.Instantiate(page, document.transform));
            }
            
            return document;
        }

#if UNITY_EDITOR
        public static FigmaDocument InstantiatePrefab(FigmaDocument original, Transform parent = null)
        {
            var documentGo = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(original.gameObject, parent);
            var document = documentGo.GetComponent<FigmaDocument>();
            
            document._pages.Clear();
            foreach (var page in original.pages)
            {
                var pageGo = (GameObject) UnityEditor.PrefabUtility.InstantiatePrefab(page.gameObject, document.transform);
                document._pages.Add(pageGo.GetComponent<FigmaPage>());
            }
            
            return document;
        }
#else
        public static FigmaDocument InstantiatePrefab(FigmaDocument original, Transform parent = null)
        {
            throw new System.NotImplementedException();
        }
#endif
        

        public IEnumerable<FigmaImporterLogReference> GetLogs()
        {
            var allLogs = Enumerable.Empty<FigmaImporterLogReference>();

            foreach (var page in pages)
            {
                allLogs = allLogs.Concat(page.allLogs);
            }
            
            return allLogs;
        }
    }
}