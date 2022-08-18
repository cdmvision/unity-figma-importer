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
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : UnityEngine.Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}