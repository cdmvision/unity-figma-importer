using Cdm.Figma.UI;
using UnityEngine;

public class InstantiateFigmaDesign : MonoBehaviour
{
    public Canvas canvas;
    public FigmaDesign design;

    private void Awake()
    {
        foreach (var page in design.pages)
        {
            Instantiate(page, canvas.transform);  
        }
    }
}