using Cdm.Figma.UI.Styles;

namespace Cdm.Figma.UI
{
    public abstract class EffectConverter : IEffectConverter
    {
        public abstract bool CanConvert(FigmaNode node, Effect effect);
        public abstract void Convert(FigmaNode node, Effect effect);
    }
    
    public abstract class EffectConverter<TEffectType, TStyle, TStyleSetter, TBehaviour> : EffectConverter 
        where TEffectType : Effect
        where TStyleSetter : StyleSetterWithSelectorsBase
        where TStyle : EffectStyle<TStyleSetter>
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
                var effectId = node.GetComponents<TBehaviour>().Length;
                
                style.effectId = effectId;
                node.styles.Add(style);
                
                var effectBehaviour = Convert(node, style);
                effectBehaviour.effectId = effectId;
            }
        }

        protected abstract TStyle CreateStyle(FigmaNode node, TEffectType effect);
        protected abstract TBehaviour Convert(FigmaNode node, TStyle style);
    }
}