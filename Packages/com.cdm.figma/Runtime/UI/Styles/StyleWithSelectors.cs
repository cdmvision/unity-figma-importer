using System;

namespace Cdm.Figma.UI.Styles
{
    public abstract class StyleWithSelectors<T> : Style where T : StylePropertyBlock, new()
    {
        public T normal = new T();
        public T highlighted = new T();
        public T pressed = new T();
        public T selected = new T();
        public T disabled = new T();
        
        public override void CopyTo(Style other)
        {
            base.CopyTo(other);

            if (other is StyleWithSelectors<T> o)
            {
                normal.CopyTo(o.normal);
                highlighted.CopyTo(o.highlighted);
                pressed.CopyTo(o.pressed);
                selected.CopyTo(o.selected);
                disabled.CopyTo(o.disabled);
            }
        }
        
        protected override void Apply(StyleArgs args)
        {
            switch (args.selector)
            {
                case Selector.Normal:
                    normal.SetStyleIfEnabled(gameObject, args);
                    break;
                case Selector.Highlighted:
                    SetStyleOrNormal(args, highlighted);
                    break;
                case Selector.Pressed:
                    SetStyleOrNormal(args, pressed);
                    break;
                case Selector.Selected:
                    SetStyleOrNormal(args, selected);
                    break;
                case Selector.Disabled:
                    SetStyleOrNormal(args, disabled);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }    
        }

        private void SetStyleOrNormal(StyleArgs args, T styleBlock)
        {
            if (!styleBlock.SetStyleIfEnabled(gameObject, args))
            {
                normal.SetStyleIfEnabled(gameObject, args);
            }
        }
    }
}