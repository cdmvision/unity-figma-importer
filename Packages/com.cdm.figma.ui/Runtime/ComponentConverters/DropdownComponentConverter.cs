using System;
using TMPro;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class DropdownComponentConverter : SelectableComponentConverter<TMP_Dropdown, SelectableComponentVariantFilter>
    {
        private const string TemplateKey = "@Template";
        private const string CaptionTextKey = "@CaptionText";
        private const string ItemTextKey = "@ItemText";
        private const string ContentItemKey = "@ContentItem";

        protected override bool CanConvertType(string typeID)
        {
            return typeID == "Dropdown";
        }

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);

            if (nodeObject != null)
            {
                var dropdown = nodeObject.GetComponent<TMP_Dropdown>();
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

                var contentItem = nodeObject.Find(x => x.bindingKey == ContentItemKey);
                if (contentItem == null)
                    throw new ArgumentException($"Dropdown content item node could not be found. Did you set '{ContentItemKey}' as binding key?");

                var scrollView = template.GetComponent<ScrollRect>();
                contentItem.transform.SetParent(scrollView.content);
            }
            
            return nodeObject;
        }
    }
}