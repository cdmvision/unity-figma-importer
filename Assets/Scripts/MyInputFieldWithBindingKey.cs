﻿using Cdm.Figma.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[FigmaNode("MyInputField")]
public class MyInputFieldWithBindingKey : MonoBehaviour
{
    [FigmaNode("@ClearButton")]
    [SerializeField]
    private Button _clearButton;

    public Button clearButton => _clearButton;

    [FigmaNode("@InputField")]
    [SerializeField]
    private TMP_InputField _inputField;

    public TMP_InputField inputField => _inputField;

    protected virtual void Start()
    {
        clearButton.onClick.AddListener(() => inputField.text = "");
    }
}