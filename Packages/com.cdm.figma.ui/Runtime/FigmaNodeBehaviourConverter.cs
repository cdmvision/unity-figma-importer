using System;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaNodeBehaviourConverter : NodeConverter
    {
        public Type type { get; }
        public string bindingKey { get; }

        public FigmaNodeBehaviourConverter(string bindingKey, Type type)
        {
            this.bindingKey = bindingKey;
            this.type = type;
        }
    
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            return bindingKey == node.GetBindingKey();
        }

        public override FigmaNode Convert(FigmaNode parentObject, Node node, NodeConvertArgs args)
        {
            if (args.importer.TryConvertNode(parentObject, node, args, out var nodeObject))
            {
                Debug.Assert(typeof(FigmaBehaviour).IsAssignableFrom(type));

                var figmaBehaviour = (FigmaBehaviour)nodeObject.gameObject.AddComponent(type);
                figmaBehaviour.Resolve();
                return nodeObject;
            }

            return null;
        }
    }
}