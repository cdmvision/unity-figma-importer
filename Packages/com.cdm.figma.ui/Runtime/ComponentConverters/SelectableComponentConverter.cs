using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public static class ComponentPropertyState
    {
        public const string Key = "State";
        public static readonly ComponentProperty Default = new ComponentProperty(Key, "Default");
        public static readonly ComponentProperty Hover = new ComponentProperty(Key, "Hover");
        public static readonly ComponentProperty Press = new ComponentProperty(Key, "Press");
        public static readonly ComponentProperty Disabled = new ComponentProperty(Key, "Disabled");
    }

    public static class ComponentPropertySelected
    {
        public const string Key = "Selected";
        public static readonly ComponentProperty Off = new ComponentProperty(Key, "Off");
        public static readonly ComponentProperty On = new ComponentProperty(Key, "On");
    }

    public abstract class SelectableComponentConverter<TComponent, TComponentVariantFilter> :
        ComponentConverter<TComponent, TComponentVariantFilter>
        where TComponent : Selectable
        where TComponentVariantFilter : SelectableComponentVariantFilter
    {
        private bool isSelectable => variants.ContainsKey(ComponentPropertySelected.Key);
        
        protected override NodeObject Convert(NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);
            var variantFilter = nodeObject.GetComponent<TComponentVariantFilter>();
            variantFilter.isSelectable = isSelectable;
            return nodeObject;
        }

        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            selector = "";

            if (!TryGetSelector(variant, ComponentPropertyState.Default, ref selector) &&
                !TryGetSelector(variant, ComponentPropertyState.Hover, ref selector) &&
                !TryGetSelector(variant, ComponentPropertyState.Press, ref selector) &&
                !TryGetSelector(variant, ComponentPropertyState.Disabled, ref selector))
            {
                return false;
            }

            if (isSelectable)
            {
                if (!TryGetSelector(variant, ComponentPropertySelected.Off, ref selector) &&
                    !TryGetSelector(variant, ComponentPropertySelected.On, ref selector))
                {
                    return false;
                }
            }

            return true;
        }
    }
}