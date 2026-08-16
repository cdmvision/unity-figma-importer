using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyBase
    {
        public bool enabled = false;
        
        public virtual void CopyTo(StylePropertyBase other)
        {
            other.enabled = enabled;
        }

        public virtual bool IsSameValue(StylePropertyBase other)
        {
            return false;
        }
    }
    
    [Serializable]
    public class StyleProperty<TValue> : StylePropertyBase
    {
        [SerializeField]
        private TValue _value = default;

        public TValue value
        {
            get => _value;
            set => _value = value;
        }

        public StyleProperty()
        {
        }

        public StyleProperty(TValue defaultValue)
        {
            _value = defaultValue;
        }

        public override void CopyTo(StylePropertyBase other)
        {
            base.CopyTo(other);

            if (other is StyleProperty<TValue> otherProperty)
            {
                otherProperty._value = _value;
            }
        }

        /// <summary>
        /// Sets the property value and enables automatically.
        /// </summary>
        public void SetValue(TValue v)
        {
            enabled = true;
            value = v;
        }

        public override bool IsSameValue(StylePropertyBase other)
        {
            return IsSameValue((StyleProperty<TValue>) other);
        }

        public virtual bool IsSameValue(StyleProperty<TValue> other)
        {
            return EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }
    }
}