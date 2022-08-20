using Cdm.Figma.UI;
using UnityEngine;

public class InstantiatePageNode : MonoBehaviour
{
    public Canvas canvas;
    public FigmaPage page;

    private void Awake()
    {
        if (page != null)
        {
            Instantiate(page, canvas.transform);    
        }
        else
        {
            Debug.LogWarning("Page is not assigned.");
        }
    }
}