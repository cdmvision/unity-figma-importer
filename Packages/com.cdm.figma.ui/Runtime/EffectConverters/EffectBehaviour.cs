using UnityEngine;

namespace Cdm.Figma.UI
{
    public abstract class EffectBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
        }
        
        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
        }        
        protected virtual void OnDisable()
        {
        }
        
        protected virtual void OnDestroy()
        {
        }

        protected abstract void UpdateEffect();
    }
}