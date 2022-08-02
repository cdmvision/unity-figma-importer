using System;
using Cdm.Figma.UI.Styles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class InputFieldComponentConverter :
        SelectableComponentConverter<TMP_InputField, SelectableComponentVariantFilter>
    {
        private const string TextViewportKey = "@TextViewport";
        private const string TextComponentKey = "@Text";
        private const string PlaceholderKey = "@Placeholder";
        
        protected override bool CanConvertType(string typeID)
        {
            return typeID == "InputField";
        }
        
        protected override NodeObject Convert(NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);

            if (nodeObject != null)
            {
                var inputField = nodeObject.GetComponent<TMP_InputField>();

                var textViewport = nodeObject.Find(x => x.bindingKey == TextViewportKey);
                if (textViewport == null)
                    throw new ArgumentException(
                        $"Text viewport node could not be found. Did you set '{TextViewportKey}' as binding key?");

                var textComponent = nodeObject.Find(x => x.bindingKey == TextComponentKey);
                if (textComponent == null)
                    throw new ArgumentException(
                        $"Text component node could not be found. Did you set '{TextComponentKey}' as binding key?");

                foreach (var style in textComponent.styles)
                {
                    if (style is TextStyle textStyle)
                    {
                        textStyle.text.enabled = false;
                    }
                }
                
                var text = textComponent.GetComponent<TMP_Text>();
                if (text == null)
                    throw new ArgumentException(
                        $"Invalid text component type!. Did you set '{TextComponentKey}' to a text node?");
                
                inputField.textViewport = textViewport.rectTransform;
                inputField.textComponent = text;

                var placeholder = nodeObject.Find(x => x.bindingKey == PlaceholderKey);
                if (placeholder != null)
                {
                    var placeholderGraphic = placeholder.GetComponent<Graphic>();
                    if (placeholderGraphic != null)
                    {
                        inputField.placeholder = placeholderGraphic;
                    }
                    else
                    {
                        Debug.LogWarning($"Text placeholder node needs to have a {nameof(Graphic)} component.");
                    }
                }

                // Force to refresh input field.
                inputField.enabled = false;
                inputField.enabled = true;
            }

            return nodeObject;
        }
    }
}