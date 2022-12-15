using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    public class FigmaDesignPreviewSceneStage : PreviewSceneStage
    {
        public FigmaDesign figmaDesign { get; set; }

        private GameObject _canvas;

        protected override GUIContent CreateHeaderContent()
        {
            return new GUIContent(figmaDesign != null ? figmaDesign.name : "Figma Design");
        }

        protected override bool OnOpenStage()
        {
            _canvas = new GameObject($"Canvas - {figmaDesign.name}");

            var canvas = _canvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            StageUtility.PlaceGameObjectInCurrentStage(_canvas);

            var designGo = (GameObject)PrefabUtility.InstantiatePrefab(figmaDesign.gameObject, _canvas.transform);

            foreach (var page in figmaDesign.document.pages)
            {
                PrefabUtility.InstantiatePrefab(page, designGo.transform);
            }

            designGo.GetComponent<FigmaDocument>().InitPages();

            return true;
        }

        protected override void OnCloseStage()
        {
            DestroyImmediate(_canvas);

            base.OnCloseStage();
        }
    }
}