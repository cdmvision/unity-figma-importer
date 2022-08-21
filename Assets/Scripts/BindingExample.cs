using Cdm.Figma.UI;
using UnityEngine;

public class BindingExample : MonoBehaviour
{
    public Canvas canvas;
    public FigmaDesign figmaDesign;

    private void Awake()
    {
        figmaDesign.CreateInstance<MyWindow>(canvas.transform);
    }
}