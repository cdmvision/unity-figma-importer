using System;
using Cdm.Figma.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectWindow : FigmaBehaviour
{
    [FigmaNode(binding = "myButton")]
    public Button myButton;
    
    [FigmaNode]
    public TMP_Text myText;
    
    [FigmaNode(binding = "myInputField")]
    public TMP_InputField myInputField;
    
    private Button _myPrivateButton;
    private Button _myPrivateButtonProperty { get; set; }

    private void Start()
    {
        if (myButton != null)
        {
            myButton.onClick.AddListener(() => Debug.Log("Hi! I was clicked!"));
        }
    }
}