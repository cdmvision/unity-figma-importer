using System;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class CompoundComponentConverter : ComponentConverter
    {
        public Type type { get; }
        public string typeId { get; }

        public CompoundComponentConverter(string typeId, Type type)
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
                Debug.Assert(typeof(FigmaBehaviour).IsAssignableFrom(type));

                var figmaBehaviour = (FigmaBehaviour)nodeObject.gameObject.AddComponent(type);
                figmaBehaviour.Resolve();
                return nodeObject;
            }

            return null;
        }
    }

    public class CompoundComponentConverter<T> : CompoundComponentConverter
    {
        public CompoundComponentConverter(string typeId) : base(typeId, typeof(T))
        {
        }
    }
}