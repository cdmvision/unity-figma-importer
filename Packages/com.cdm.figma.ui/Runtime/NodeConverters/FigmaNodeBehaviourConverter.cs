using System;
using Cdm.Figma.UI.Utils;

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
            if (args.importer.TryConvertNode(parentObject, node, args, out var nodeObject, this))
            {
                nodeObject.Bind(type, args);
                return nodeObject;
            }

            return null;
        }
    }

    public class FigmaNodeBehaviourConverter<T> : FigmaNodeBehaviourConverter
        where T : UnityEngine.Component
    {
        public FigmaNodeBehaviourConverter(string bindingKey) : base(bindingKey, typeof(T))
        {
        }
    }
}