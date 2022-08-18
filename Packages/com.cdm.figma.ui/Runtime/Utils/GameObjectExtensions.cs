using UnityEngine;

namespace Cdm.Figma.UI.Utils
{
    public static class ObjectUtils
    {
        public static void Destroy(Object obj)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                Object.Destroy(obj);
#if UNITY_EDITOR
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
#endif
        }
    }
}