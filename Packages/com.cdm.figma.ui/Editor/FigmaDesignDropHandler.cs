using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    [InitializeOnLoad]
    public static class FigmaDesignDropHandler
    {
        static FigmaDesignDropHandler()
        {
            DragAndDrop.AddDropHandler(OnHierarchyDropHandler);
            DragAndDrop.AddDropHandler(OnSceneDropHandler);
        }

        [OnOpenAsset(0)]
        public static bool OnOpenFigmaDesign(int instanceID, int line)
        {
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go != null)
            {
                var figmaDesign = go.GetComponent<FigmaDesign>();
                if (figmaDesign != null)
                {
                    FigmaDesignPreviewStage.Show(figmaDesign);
                    return true;
                }

                var figmaPage = go.GetComponent<FigmaPage>();
                if (figmaPage != null)
                {
                    FigmaDesignPreviewStage.Show(figmaPage);
                    return true;
                }
            }

            return false;
        }

        private static DragAndDropVisualMode OnHierarchyDropHandler(
            int instanceId,
            HierarchyDropFlags dropMode,
            Transform parentForDraggedObjects,
            bool perform)
        {
            var go = DragAndDrop.objectReferences[0] as GameObject;
            if (go != null)
            {
                var figmaDesign = go.GetComponent<FigmaDesign>();
                if (figmaDesign != null)
                {
                    if (perform)
                    {
                        var parentObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                        InstantiateFigmaDesign(figmaDesign, parentObject);
                    }

                    return DragAndDropVisualMode.Copy;
                }
            }

            return DragAndDropVisualMode.None;
        }

        private static DragAndDropVisualMode OnSceneDropHandler(
            Object dropUpon,
            Vector3 worldPosition,
            Vector2 viewportPosition,
            Transform parentForDraggedObjects,
            bool perform)
        {
            var go = DragAndDrop.objectReferences[0] as GameObject;
            if (go != null)
            {
                var figmaDesign = go.GetComponent<FigmaDesign>();
                if (figmaDesign != null)
                {
                    if (perform)
                    {
                        var figmaDesignInstance = InstantiateFigmaDesign(figmaDesign, dropUpon as GameObject);
                        if (figmaDesignInstance != null)
                        {
                            figmaDesignInstance.transform.position = worldPosition;
                        }
                    }

                    return DragAndDropVisualMode.Copy;
                }
            }

            return DragAndDropVisualMode.None;
        }

        private static GameObject InstantiateFigmaDesign(FigmaDesign figmaDesign, GameObject parentObject)
        {
            var parent = parentObject != null ? parentObject.transform : null;
            var figmaDesignGo = (GameObject)PrefabUtility.InstantiatePrefab(figmaDesign.gameObject, parent);

            foreach (var page in figmaDesign.document.pages)
            {
                PrefabUtility.InstantiatePrefab(page, figmaDesignGo.transform);
            }

            figmaDesignGo.GetComponent<FigmaDocument>().InitPages();
            return figmaDesignGo;
        }
    }
}