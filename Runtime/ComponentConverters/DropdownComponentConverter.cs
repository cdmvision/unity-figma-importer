using Cdm.Figma.UI.Utils;
using TMPro;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Cdm.Figma.UI
{
    public abstract class DropdownComponentConverter<T> : ComponentConverter where T : TMP_Dropdown
    {
        protected const string TemplateKey = BindingPrefix + "Template";
        protected const string CaptionTextKey = BindingPrefix + "CaptionText";
        protected const string CaptionImageKey = BindingPrefix + "CaptionImage";
        protected const string ContentItemKey = BindingPrefix + "ContentItem";
        protected const string ContentItemTextKey = BindingPrefix + "ContentItemText";
        protected const string ContentItemImageKey = BindingPrefix + "ContentItemImage";
        
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

                var dropdown = figmaNode.gameObject.AddComponent<T>();
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

                if (figmaNode.TryFindOptionalNode<Image>(CaptionImageKey, out var captionImage))
                {
                    dropdown.captionImage = captionImage;
                }

                var scrollView = template.GetComponent<ScrollRect>();
                contentItem.transform.SetParent(scrollView.content);
            }

            return figmaNode;
        }
    }
    
    public class DropdownComponentConverter : DropdownComponentConverter<TMP_Dropdown>
    {
        protected override bool CanConvertType(string typeId)
        {
            return typeId == "Dropdown";
        }
    }
}