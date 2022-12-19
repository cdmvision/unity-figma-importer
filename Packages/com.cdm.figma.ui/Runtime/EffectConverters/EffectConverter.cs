namespace Cdm.Figma.UI
{
    public abstract class EffectConverter : IEffectConverter
    {
        public abstract bool CanConvert(FigmaNode node, Effect effect);
        public abstract void Convert(FigmaNode node, Effect effect);
    }
    
    public abstract class EffectConverter<TEffectType, TStyle, TBehaviour> : EffectConverter 
        where TEffectType : Effect
        where TStyle : Styles.Style
        where TBehaviour: EffectBehaviour
    {
        public override bool CanConvert(FigmaNode node, Effect effect)
        {
            return effect is TEffectType;
        }

        public override void Convert(FigmaNode node, Effect effect)
        {
            var style = CreateStyle(node, (TEffectType)effect);
            if (style != null)
            {
                node.styles.Add(style);
                Convert(node, style);
            }
        }

        protected abstract TStyle CreateStyle(FigmaNode node, TEffectType effect);
        protected abstract TBehaviour Convert(FigmaNode node, TStyle style);
    }
}