using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Utils;
using TMPro;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class DropdownComponentConverter : ComponentConverter
    {
        private const string TemplateKey = BindingPrefix + "Template";
        private const string CaptionTextKey = BindingPrefix + "CaptionText";
        private const string ContentItemKey = BindingPrefix + "ContentItem";
        private const string ContentItemTextKey = BindingPrefix + "ContentItemText";
        private const string ContentItemImageKey = BindingPrefix + "ContentItemImage";

        protected override bool CanConvertType(string typeId)
        {
            return typeId == "Dropdown";
        }

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var figmaNode = base.Convert(parentObject, instanceNode, args);
            if (figmaNode != null)
            {
                if (!figmaNode.TryFindNode(args, TemplateKey, out var template) ||
                    !figmaNode.TryFindNode(args, ContentItemKey, out var contentItem))
                {
                    return figmaNode;
                }

                var dropdown = figmaNode.gameObject.AddComponent<TMP_Dropdown>();
                dropdown.transition = Selectable.Transition.None;

                template.gameObject.SetActive(true);
                contentItem.gameObject.SetActive(true);

                dropdown.template = template.rectTransform;
                dropdown.template.gameObject.SetActive(false);

                if (contentItem.TryFindOptionalNode<TMP_Text>(ContentItemTextKey, out var itemText))
                {
                    dropdown.itemText = itemText;
                    dropdown.itemText.DisableTextStyleTextOverride();
                }

                if (contentItem.TryFindOptionalNode<Image>(ContentItemImageKey, out var itemImage))
                {
                    dropdown.itemImage = itemImage;
                }

                if (figmaNode.TryFindOptionalNode<TMP_Text>(CaptionTextKey, out var captionText))
                {
                    dropdown.captionText = captionText;
                    dropdown.captionText.DisableTextStyleTextOverride();
                }

                var scrollView = template.GetComponent<ScrollRect>();
                contentItem.transform.SetParent(scrollView.content);
            }

            return figmaNode;
        }
    }
}