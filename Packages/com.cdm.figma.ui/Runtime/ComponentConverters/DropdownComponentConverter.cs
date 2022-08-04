using System;
using TMPro;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class DropdownComponentConverter : ComponentConverter
    {
        private const string TemplateKey = "@Template";
        private const string CaptionTextKey = "@CaptionText";
        private const string ItemTextKey = "@ItemText";
        
        protected override bool CanConvertType(string typeID)
        {
            return typeID == "Dropdown";
        }

        protected override NodeObject Convert(NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);

            if (nodeObject != null)
            {
                var dropdown = nodeObject.gameObject.AddComponent<TMP_Dropdown>();
                dropdown.transition = Selectable.Transition.None;

                var template = nodeObject.Find(x => x.bindingKey == TemplateKey);
                if(template == null)
                    throw new ArgumentException($"Dropdown template node could not be found. Did you set '{TemplateKey}' as binding key?");

                dropdown.template = template.rectTransform;
                dropdown.template.gameObject.SetActive(false);
                
                var captionText = nodeObject.Find(x => x.bindingKey == CaptionTextKey);
                if (captionText != null)
                {
                    dropdown.captionText = captionText.GetComponent<TextMeshProUGUI>();
                }
            }
            
            return nodeObject;
        }

        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            selector = "";
            return false;
        }
    }
}