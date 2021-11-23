using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(UIToolkitFigmaImporter), 
        menuName = AssetMenuRoot + "Figma Importer", order = 0)]
    public class UIToolkitFigmaImporter : FigmaImporter
    {
        public new const string AssetMenuRoot = FigmaImporter.AssetMenuRoot + "UI Toolkit/";

        [SerializeField]
        private string _typePrefix = "/t:";

        public string typePrefix
        {
            get => _typePrefix;
            set => _typePrefix = value;
        }
        
        [SerializeField]
        private string _bindingPrefix = "/b:";

        public string bindingPrefix
        {
            get => _bindingPrefix;
            set => _bindingPrefix = value;
        }
        
        [SerializeField]
        private string _localizationPrefix = "/l:";

        public string localizationPrefix
        {
            get => _localizationPrefix;
            set => _localizationPrefix = value;
        }
        
        [SerializeField]
        private List<ComponentConverter> _converters = new List<ComponentConverter>();

        public List<ComponentConverter> converters => _converters;

        public override Task ImportFileAsync(FigmaFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            // TODO: Implement
            
            Debug.Log("TODO: IMPORT_FILE_ASYNC");
            return Task.CompletedTask;
        }
    }
}