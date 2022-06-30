using System;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class Style
    {
        [SerializeField]
        private string _selector;

        public string selector
        {
            get => _selector;
            set => _selector = value;
        }
        
        [SerializeField]
        public bool _enabled = false;

        public bool enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public virtual void CopyTo(Style other)
        {
            other._enabled = _enabled;
            other._selector = _selector;
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

        public virtual void SetStyle(GameObject gameObject, StyleArgs args)
        {
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