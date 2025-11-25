using System.Linq;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    public abstract class EffectStyle<T> : StyleWithSetter<T> where T : StyleSetterWithSelectorsBase
    {
        [SerializeField]
        private int _effectId;

        public int effectId
        {
            get => _effectId;
            set => _effectId = value;
        }
        
        protected TEffectBehaviour GetEffectBehaviour<TEffectBehaviour>(GameObject gameObject) 
            where TEffectBehaviour : EffectBehaviour
        {
            return gameObject.GetComponents<TEffectBehaviour>().FirstOrDefault(c => c.effectId == effectId);
        }
    }
}