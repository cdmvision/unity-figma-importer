using Cdm.Figma.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[FigmaNode("myWindow")]
public class MyWindow : FigmaBehaviour
{
    // Public property, setter can be private.
    [FigmaNode]
    public Button myButton { get; private set; }
    
    // Public field.
    [FigmaNode]
    public TMP_Text myText;
    
    // Private field with binding key that is set explicitly.
    [FigmaNode("myInputField")]
    private TMP_InputField _myInputField;

    public GameObject go;
    
    protected override void Start()
    {
        base.Start();

        myButton.onClick.AddListener(() => Debug.Log($"Text: {_myInputField.text}"));
        
        _myInputField.onValueChanged.AddListener(str =>
        {
            myText.text = str;
        });
    }
}