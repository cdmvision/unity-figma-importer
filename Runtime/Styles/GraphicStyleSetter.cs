using Cdm.Figma.UI.Styles.Properties;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    public class GraphicStyleSetter : StyleSetterWithSelectors<GraphicStyle>
    {
    }
    
    public static class GraphicExtensions
    {
        public static void CrossFadeColor(this Graphic graphic, StyleArgs args, 
            StylePropertyColor color, StylePropertyFloat fadeDuration)
        {
            var duration = !fadeDuration.enabled || args.instant ? 0f : fadeDuration.value;
            graphic.CrossFadeColor(color.value, duration, false, true);    
        }
    }
}