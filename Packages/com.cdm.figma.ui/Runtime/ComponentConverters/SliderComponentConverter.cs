using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class SliderComponentConverter : SelectableComponentConverter<Slider, SelectableComponentVariantFilter>
    {
        protected override bool CanConvertType(string typeID)
        {
            return typeID == "Slider";
        }

        protected override NodeObject Convert(NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);
            
            if (nodeObject != null)
            {
                var slider = nodeObject.GetComponent<Slider>();

                
                slider.fillRect = (RectTransform) Find(slider.transform, "Fill");
                slider.handleRect = (RectTransform) Find(slider.transform, "Handle");
            }

            return nodeObject;
        }

        private static Transform Find(Transform target, string name)
        {
            if (target.name == name)
                return target;

            foreach (Transform child in target)
            {
                var t = Find(child, name);
                if (t != null)
                    return t;
            }

            return null;
        }
    }
}