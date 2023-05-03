using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Editor
{
    public class FigmaDesignPreviewStage : PreviewSceneStage
    {
        private FigmaDesign _design;
        private FigmaPage _page;
        private GameObject _canvas;

        public static void Show(FigmaDesign design)
        {
            Show(design, null);
        }

        public static void Show(FigmaPage page)
        {
            Show(null, page);
        }

        private static void Show(FigmaDesign design, FigmaPage page)
        {
            var stage = CreateInstance<FigmaDesignPreviewStage>();
            stage._design = design;
            stage._page = page;
            StageUtility.GoToStage(stage, true);
        }

        protected override GUIContent CreateHeaderContent()
        {
            return new GUIContent(GetNodeName());
        }

        protected override bool OnOpenStage()
        {
            scene = EditorSceneManager.NewPreviewScene();
            
            _canvas = new GameObject($"Canvas");

            var canvas = _canvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            _canvas.AddComponent<CanvasScaler>();

            StageUtility.PlaceGameObjectInCurrentStage(_canvas);

            var success = false;
            if (_design != null)
            {
                FigmaDocument.InstantiatePrefab(_design.document, _canvas.transform);
                success = true;
            }

            if (_page != null)
            {
                PrefabUtility.InstantiatePrefab(_page.gameObject, _canvas.transform);
                success = true;
            }

            if (success)
            {
                Selection.objects = new Object[] { _canvas };
                SceneView.FrameLastActiveSceneView();
            }
            else
            {
                DestroyImmediate(_canvas);
            }
            
            return success;
        }

        protected override void OnCloseStage()
        {
            if (scene.IsValid())
            {
                EditorSceneManager.ClosePreviewScene(scene);
            }
            
            base.OnCloseStage();
        }

        private string GetNodeName()
        {
            if (_design != null)
                return _design.name;

            if (_page != null)
                return $"{_page.figmaDesign.name}/{_page.name}";

            return "";
        }
    }
}