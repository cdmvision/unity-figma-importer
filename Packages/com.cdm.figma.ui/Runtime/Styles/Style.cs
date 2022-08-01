using System;
using Cdm.Figma.UI.Styles.Properties;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public abstract class Style
    {
        [SerializeField]
        private string _selector;

        public string selector
        {
            get => _selector;
            set => _selector = value;
        }
        
        [SerializeField]
        public bool _enabled = true;

        public bool enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public void CopyTo(Style other)
        {
            other._enabled = _enabled;
            other._selector = _selector;

            MergeTo(other, true);
        }

        public virtual void MergeTo(Style other)
        {
            MergeTo(other, false);
        }

        
        public abstract void SetStyle(GameObject gameObject, StyleArgs args);
        public abstract void SetStyleAsSelector(GameObject gameObject, StyleArgs args);
        
        protected abstract void MergeTo(Style other, bool force);
        
        protected void OverwriteProperty(StylePropertyBase property, StylePropertyBase other, bool force)
        {
            if (property.enabled || force)
            {
                property.CopyTo(other);
            }
        }

        public bool SetStyleIfEnabled(GameObject gameObject, StyleArgs args)
        {
            if (_enabled)
            {
                SetStyle(gameObject, args);
                return true;
            }

            return false;
        }

        protected void SetStyleAsSelector<T>(GameObject gameObject, StyleArgs args) 
            where T : StyleSetterWithSelectorsBase
        {
            var styleSetter = GetOrAddComponent<T>(gameObject);
            if (styleSetter != null)
            {
                var existingStyle = styleSetter.GetStyle(x => x.selector == selector);
                if (existingStyle != null)
                {
                    MergeTo(existingStyle);
                }
                else
                {
                    styleSetter.AddStyle(this);
                }
            }
        }

        protected static bool TryGetComponent<T>(GameObject gameObject, out T component, bool giveWarning = true)
            where T : UnityEngine.Component
        {
            component = gameObject.GetComponent<T>();
            if (component != null)
            {
                return true;
            }

            if (giveWarning)
                Debug.LogWarning($"Component not found: {typeof(T).Name}", gameObject);

            return false;
        }

        protected static T GetOrAddComponent<T>(GameObject gameObject) where T : UnityEngine.Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}