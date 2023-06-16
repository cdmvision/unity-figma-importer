using System;
using System.Linq;
using Cdm.Figma.UI.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cdm.Figma.UI
{
    public static class FigmaDesignExtensions
    {
        public static T CreateInstance<T>(this FigmaDesign figmaDesign, string bindingKey, Transform parent = null)
            where T : UnityEngine.Component
        {
            if (string.IsNullOrEmpty(bindingKey))
                throw new ArgumentNullException(nameof(bindingKey), "Binding key cannot be empty.");

            var figmaNode = figmaDesign.Query<FigmaNode>(bindingKey);
            if (figmaNode != null)
            {
                var component = Object.Instantiate(figmaNode, parent);
                
                // Bind component if it is not FigmaBehaviour. If so, FigmaBehaviour will bind it automatically.
                if (!typeof(FigmaBehaviour).IsAssignableFrom(typeof(T)))
                {
                    var bindingResult = FigmaNodeBinder.Bind(component, figmaNode);
                    if (bindingResult.hasErrors)
                        throw new FigmaBindingFailedException(bindingResult.errors);
                }
                
                return component.gameObject.AddComponent<T>();
            }

            return null;
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