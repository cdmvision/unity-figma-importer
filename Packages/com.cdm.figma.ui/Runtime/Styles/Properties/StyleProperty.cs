using System;
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
    }
}