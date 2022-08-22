using Cdm.Figma.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[FigmaComponent("InputFieldWithClearButton")]
public class CustomInputField : MonoBehaviour, IFigmaNodeBinder
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

    public void OnBind(FigmaNode figmaNode)
    {
        Debug.Log($"{figmaNode} I'm binding some data!");
    }
}