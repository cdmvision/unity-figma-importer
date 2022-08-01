using System.Collections.Generic;
using Cdm.Figma.UI.Styles;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cdm.Figma.UI
{
    public abstract class ComponentVariantFilter : UIBehaviour
    {
        [SerializeField]
        private List<StyleSetter> _stylesSetters = new List<StyleSetter>();
        
        protected abstract string GetSelector();
        
        public virtual void Initialize()
        {
            _stylesSetters.Clear();
            Initialize(transform);
        }

        protected void UpdateVariant()
        {
            SetSelectorForStyles(GetSelector());
        }
        
        private void Initialize(Transform node)
        {
            var variantFilter = node.GetComponent<ComponentVariantFilter>();
            if (variantFilter == null || variantFilter == this)
            {
                var styleSetters = node.GetComponents<StyleSetter>();
                _stylesSetters.AddRange(styleSetters);
            }

            foreach (Transform child in node)
            {
                Initialize(child);
            }
        }
        
        private void SetSelectorForStyles(string selector)
        {
            foreach (var style in _stylesSetters)
            {
                if (style.enabled)
                {
                    style.Apply(selector);    
                }
            }
        }
    }
}