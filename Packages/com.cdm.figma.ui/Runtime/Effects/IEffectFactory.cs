using UnityEngine;

namespace Cdm.Figma.UI.Effects
{
    public interface IEffectFactory
    {
        bool Add(GameObject gameObject, Styles.Style style);
    }
}