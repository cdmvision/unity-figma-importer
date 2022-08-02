using UnityEngine;

namespace Cdm.Figma.UI.Utils
{
    public static class ObjectUtils
    {
        public static void Destroy(GameObject go)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                Object.Destroy(go);
#if UNITY_EDITOR
            }
            else
            {
                Object.DestroyImmediate(go);
            }
#endif
        }
    }
}