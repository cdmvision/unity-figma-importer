using System;
using Cdm.Figma.UI.Styles;
using TMPro;
using UnityEngine;

namespace Cdm.Figma.UI.Utils
{
    public static class NodeConverterExtensions
    {
        public static void Bind(this FigmaNode nodeObject, Type type, NodeConvertArgs args)
        {
            Debug.Assert(typeof(UnityEngine.Component).IsAssignableFrom(type));

            var component = nodeObject.gameObject.AddComponent(type);

            var bindingResult = FigmaNodeBinder.Bind(component, nodeObject);
            if (bindingResult.hasErrors)
            {
                if (args.importer.failOnError)
                    throw new FigmaBindingFailedException(bindingResult.errors);
                    
                foreach (var error in bindingResult.errors)
                {
                    args.importer.LogError(error.ToString(), nodeObject);    
                }
            }
        }
        
        public static void DisableTextStyleTextOverride(this TMP_Text node)
        {
            DisableTextStyleTextOverride(node.GetComponent<FigmaNode>());
        }
        
        public static void DisableTextStyleTextOverride(this FigmaNode node)
        {
            foreach (var style in node.styles)
            {
                if (style is TextStyle textStyle)
                {
                    textStyle.text.enabled = false;
                }
            }
        }
        
        public static bool TryFindNode(this FigmaNode node, NodeConvertArgs args, string bindingKey, 
            out FigmaNode target)
        {
            return TryFindNode<FigmaNode>(node, args, bindingKey, out target);
        }

        public static bool TryFindOptionalNode(this FigmaNode node, string bindingKey, out FigmaNode target)
        {
            return TryFindOptionalNode<FigmaNode>(node, bindingKey, out target);
        }

        public static bool TryFindNode<T>(this FigmaNode node, NodeConvertArgs args, string bindingKey, out T target) 
            where T : UnityEngine.Component
        {
            var targetNode = node.Find(x => x.bindingKey == bindingKey);
            if (targetNode != null)
            {
                target = targetNode.GetComponent<T>();
                if (target == null)
                {
                    
                    args.importer.LogError($"'{bindingKey}' might be assigned to wrong node. "+ 
                                           $"'{typeof(T).FullName}' component is required.", node);
                    
                    return false;
                }
                return true;
            }

            args.importer.LogError($"'{bindingKey}' could not be found.", node);
            target = null;
            return false;
        }

        public static bool TryFindOptionalNode<T>(this FigmaNode node, string bindingKey, out T target) 
            where T : UnityEngine.Component
        {
            var targetNode = node.Find(x => x.bindingKey == bindingKey);
            if (targetNode != null)
            {
                target = targetNode.GetComponent<T>();
                return target != null;
            }

            target = null;
            return false;
        }
    }
}