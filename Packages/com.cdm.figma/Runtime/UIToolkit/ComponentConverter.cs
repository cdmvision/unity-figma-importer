using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [Serializable]
    public struct ComponentState
    {
        public string state;
        public string value;

        public ComponentState(string state, string value)
        {
            this.state = state;
            this.value = value;
        }
    }
    
    public abstract class ComponentConverter : ScriptableObject, IComponentConverter
    {
        protected const string AssetMenuRoot = UIToolkitFigmaImporter.AssetMenuRoot + "Converters/";
        
        [SerializeField]
        private string _typeId;

        public string typeId => _typeId;
        
        [SerializeField]
        private List<ComponentState> _states = new List<ComponentState>();

        private void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(_typeId))
            {
                _typeId = GetDefaultTypeId();
            }
            
            if (!_states.Any())
            {
                _states.AddRange(GetStates());
            }
        }

        protected abstract string GetDefaultTypeId();
        protected abstract ISet<ComponentState> GetStates();
        
        public bool CanConvert(UIToolkitFigmaImporter importer, FigmaFile file, Node node)
        {
            return node?.name.Contains($"{importer.typePrefix}{typeId}") ?? false;
        }

        public abstract VisualTreeAsset Convert(UIToolkitFigmaImporter importer, FigmaFile file, Node node);
    }
}