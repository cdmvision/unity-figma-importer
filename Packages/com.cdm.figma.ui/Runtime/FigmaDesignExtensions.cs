using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cdm.Figma.UI
{
    public static class FigmaDesignExtensions
    {
        public static T CreateInstance<T>(this FigmaDesign figmaDesign, string bindingKey, Transform parent = null) 
            where T : FigmaBehaviour
        {
            var figmaNode = figmaDesign.Query<FigmaNode>(bindingKey);
            if (figmaNode != null)
            {
                var figmaNodeInstance = Object.Instantiate(figmaNode, parent);
                return figmaNodeInstance.gameObject.AddComponent<T>();
            }

            return null;
        }

        public static T CreateInstance<T>(this FigmaDesign figmaDesign, Transform parent = null) 
            where T : FigmaBehaviour
        {
            var figmaNodeAttribute = 
                (FigmaNodeAttribute) Attribute.GetCustomAttribute(typeof(T), typeof(FigmaNodeAttribute));
            
            var bindingKey = typeof(T).Name;
            
            if (!string.IsNullOrEmpty(figmaNodeAttribute.bindingKey))
            {
                bindingKey = figmaNodeAttribute.bindingKey;
            }

            return figmaDesign.CreateInstance<T>(bindingKey, parent);
        }
        
        public static T Query<T>(this FigmaDesign figmaDesign, string bindingKey) where T : UnityEngine.Component
        {
            return figmaDesign.document.Query<T>(bindingKey);
        }

        public static T[] QueryAll<T>(this FigmaDesign figmaDesign, string bindingKey) where T : UnityEngine.Component
        {
            return figmaDesign.document.QueryAll<T>(bindingKey);
        }

        public static UnityEngine.Component Query(this FigmaDesign figmaDesign, string bindingKey, Type type)
        {
            return figmaDesign.document.Query(bindingKey, type);
        }

        public static UnityEngine.Component[] QueryAll(this FigmaDesign figmaDesign, string bindingKey, Type type)
        {
            return figmaDesign.document.QueryAll(bindingKey, type);
        }
    }
}