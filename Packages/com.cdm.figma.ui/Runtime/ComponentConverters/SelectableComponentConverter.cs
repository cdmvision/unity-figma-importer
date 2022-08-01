using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public abstract class SelectableComponentConverter<TComponent, TComponentVariantFilter> :
        ComponentConverter<TComponent, TComponentVariantFilter>
        where TComponent : Selectable
        where TComponentVariantFilter : SelectableComponentVariantFilter
    {
        protected const int Default = 0;
        protected const int Hover = 1;
        protected const int Press = 2;
        protected const int Disabled = 3;

        public ComponentProperty stateProperty { get; } = new ComponentProperty("State", new[]
        {
            SelectableComponentState.Default,
            SelectableComponentState.Hover,
            SelectableComponentState.Press,
            SelectableComponentState.Disabled,
        });

        public SelectableComponentConverter()
        {
            properties.Add(stateProperty);
        }

        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            if (IsSameVariant(variant, stateProperty.ToString(Default)))
            {
                selector = SelectableComponentState.Default;
                return true;
            }

            if (IsSameVariant(variant, stateProperty.ToString(Hover)))
            {
                selector = SelectableComponentState.Hover;
                return true;
            }

            if (IsSameVariant(variant, stateProperty.ToString(Press)))
            {
                selector = SelectableComponentState.Press;
                return true;
            }

            if (IsSameVariant(variant, stateProperty.ToString(Disabled)))
            {
                selector = SelectableComponentState.Disabled;
                return true;
            }

            selector = SelectableComponentState.Default;
            return false;
        }
    }
}