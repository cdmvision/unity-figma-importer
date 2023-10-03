﻿using Cdm.Figma.UI.Utils;
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
                        componentData.selectionColor.a = componentData.selectionColorOpacity / 100f;
                        inputField.selectionColor = selectionColor;

                        var caretColor = (UnityEngine.Color)componentData.caretColor;
                        componentData.caretColor.a = componentData.caretColorOpacity / 100f;
                        inputField.caretColor = caretColor;

                        inputField.caretWidth = componentData.caretWidth;
                        inputField.customCaretColor = true;
                    }
                }

                // Force to refresh input field.
                inputField.enabled = false;
                inputField.enabled = true;
            }

            return figmaNode;
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