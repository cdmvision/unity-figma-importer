﻿using Cdm.Figma.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[FigmaComponent("InputFieldWithClearButton")]
public class CustomInputField : FigmaBehaviour
{
    [FigmaNode("@ClearButton")]
    [SerializeField]
    private Button _clearButton;

    public Button clearButton => _clearButton;

    [FigmaNode("@InputField")]
    [SerializeField]
    private TMP_InputField _inputField;

    public TMP_InputField inputField => _inputField;

    protected override void Start()
    {
        base.Start();
        
        clearButton.onClick.AddListener(() => inputField.text = "");
    }
}