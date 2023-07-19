using Cdm.Figma.UI.Styles;

namespace Cdm.Figma.UI
{
    public class LeTaiShadowEffectConverter : ShadowEffectConverter
    {
        protected override ShadowEffectBehaviour Convert(FigmaNode node, ShadowStyle style)
        {
            return node.gameObject.AddComponent<LeTaiShadowEffectBehaviour>();
        }
    }
}