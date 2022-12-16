using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Editor
{
    public class FigmaDesignPreviewSceneStage : PreviewSceneStage
    {
        public FigmaDesign design { get; set; }
        public FigmaPage page { get; set; }

        private GameObject _canvas;

        protected override GUIContent CreateHeaderContent()
        {
            var title = "Figma Design";
            if (design != null)
            {
                title = design.name;
            }
            else if (page != null)
            {
                title = $"{page.figmaDesign.name}/{page.name}";
            }

            return new GUIContent(title);
        }

        protected override bool OnOpenStage()
        {
            if (design != null)
            {
                _canvas = new GameObject($"Canvas - {design.name}");    
            }
            else if (page != null)
            {
                _canvas = new GameObject($"Canvas - {page.name}");
            }

            var canvas = _canvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            _canvas.AddComponent<CanvasScaler>();

            StageUtility.PlaceGameObjectInCurrentStage(_canvas);

            if (design != null)
            {
                var designGo = (GameObject)PrefabUtility.InstantiatePrefab(design.gameObject, _canvas.transform);

                foreach (var p in design.document.pages)
                {
                    PrefabUtility.InstantiatePrefab(p.gameObject, designGo.transform);
                }

                designGo.GetComponent<FigmaDocument>().InitPages();
                return true;
            }

            if (page != null)
            {
                PrefabUtility.InstantiatePrefab(page.gameObject, _canvas.transform);
                return true;
            }

            return false;
        }

        protected override void OnCloseStage()
        {
            DestroyImmediate(_canvas);

            base.OnCloseStage();
        }
    }
}