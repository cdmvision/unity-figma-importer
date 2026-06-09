using System.Globalization;
using Cdm.Figma.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public abstract class InputFieldComponentConverter<TInputField, TComponentVariantFilter> :
        SelectableComponentConverter<TInputField, TComponentVariantFilter>
        where TInputField : TMP_InputField
        where TComponentVariantFilter : SelectableComponentVariantFilter
    {
        protected const string TextViewportKey = BindingPrefix + "TextViewport";
        protected const string TextComponentKey = BindingPrefix + "Text";
        protected const string PlaceholderKey = BindingPrefix + "Placeholder";

        /*
        * MASK_PADDING_OFFSET provides a small safety margin for the TextArea's RectMask2D.
        * Without it, the text can render at the input field’s border
        */
        private const int MASK_PADDING_OFFSET = 3;

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var figmaNode = base.Convert(parentObject, instanceNode, args);

            if (figmaNode != null)
            {
                if (!figmaNode.TryFindNode<RectTransform>(args, TextViewportKey, out var textViewport) ||
                    !figmaNode.TryFindNode<TMP_Text>(args, TextComponentKey, out var textComponent))
                {
                    return figmaNode;
                }
                
                textComponent.DisableTextStyleTextOverride();
                
                var inputField = figmaNode.GetComponent<TMP_InputField>();
                inputField.textViewport = textViewport;
                inputField.textComponent = textComponent;
                
                if (figmaNode.TryFindOptionalNode<Graphic>(PlaceholderKey, out var placeholder))
                {
                    inputField.placeholder = placeholder;
                }

                if (instanceNode.mainComponent.componentSet != null &&
                    instanceNode.mainComponent.componentSet.TryGetPluginData(out var pluginData))
                {
                    var componentData = pluginData.GetComponentDataAs<InputFieldComponentData>();
                    if (componentData != null)
                    {
                        var selectionColor = (UnityEngine.Color)componentData.selectionColor;
                        selectionColor.a = float.TryParse(componentData.selectionColorOpacity, NumberStyles.Float, CultureInfo.InvariantCulture, out var selectionOpacity)
                            ? selectionOpacity / 100f
                            : 0.75f;
                        inputField.selectionColor = selectionColor;

                        var caretColor = (UnityEngine.Color)componentData.caretColor;
                        caretColor.a = float.TryParse(componentData.caretColorOpacity, NumberStyles.Float, CultureInfo.InvariantCulture, out var caretOpacity)
                            ? caretOpacity / 100f
                            : 1f;
                        inputField.caretColor = caretColor;

                        inputField.caretWidth = componentData.caretWidth;
                        inputField.customCaretColor = true;
                    }
                }

                AppendPaddingToMask(figmaNode, textComponent, inputField);
                
                // Force to refresh input field.
                inputField.enabled = false;
                inputField.enabled = true;
            }

            return figmaNode;
        }

        private void AppendPaddingToMask(FigmaNode figmaNode, TMP_Text textComponent, TMP_InputField inputField)
        {
            float horizontalPadding = GetPadding(figmaNode.rectTransform, textComponent.rectTransform, RectTransform.Axis.Horizontal);
            var maskPaddingHorizontal = horizontalPadding - MASK_PADDING_OFFSET;
            
            float verticalPadding = GetPadding(figmaNode.rectTransform, textComponent.rectTransform, RectTransform.Axis.Vertical);
            var maskPaddingVertical = verticalPadding - MASK_PADDING_OFFSET;
            
            maskPaddingHorizontal = Mathf.Max(maskPaddingHorizontal, 0);
            maskPaddingVertical = Mathf.Max(maskPaddingVertical, 0);
            
            var mask = inputField.textViewport.gameObject.GetOrAddComponent<RectMask2D>();
            mask.padding = new Vector4(-maskPaddingHorizontal, -maskPaddingVertical, -maskPaddingHorizontal, -maskPaddingVertical);
        }
        
        private float GetPadding(RectTransform parent, RectTransform child, RectTransform.Axis axis)
        {
            Vector3[] p = new Vector3[4];
            Vector3[] c = new Vector3[4];

            parent.GetWorldCorners(p);
            child.GetWorldCorners(c);

            if(axis == RectTransform.Axis.Horizontal)
                return p[2].x - c[2].x;
            
            return c[0].y - p[0].y;
        }
    }

    public class InputFieldComponentConverter
        : InputFieldComponentConverter<TMP_InputField, SelectableComponentVariantFilter>
    {
        protected override bool CanConvertType(string typeId)
        {
            return typeId == "InputField";
        }
    }
}