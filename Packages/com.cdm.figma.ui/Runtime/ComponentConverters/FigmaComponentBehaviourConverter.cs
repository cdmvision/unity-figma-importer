using System;
using Cdm.Figma.UI.Utils;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaComponentBehaviourConverter : ComponentConverter
    {
        public Type type { get; }
        public string typeId { get; }

        public FigmaComponentBehaviourConverter(string typeId, Type type)
        {
            this.typeId = typeId;
            this.type = type;
        }

        protected override bool CanConvertType(string otherId)
        {
            return otherId == typeId;
        }

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);
            if (nodeObject != null)
            {
                Debug.Assert(typeof(UnityEngine.Component).IsAssignableFrom(type));

                var component = nodeObject.gameObject.AddComponent(type);
                FigmaNodeBinder.Bind(component, nodeObject);
                return nodeObject;
            }

            return null;
        }
    }

    public class FigmaComponentBehaviourConverter<T> : FigmaComponentBehaviourConverter 
        where T : UnityEngine.Component
    {
        public FigmaComponentBehaviourConverter(string typeId) : base(typeId, typeof(T))
        {
        }
    }
}