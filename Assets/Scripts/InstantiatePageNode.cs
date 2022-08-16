using Cdm.Figma.UI;
using UnityEngine;

public class InstantiatePageNode : MonoBehaviour
{
    public Canvas canvas;
    public PageNodeObject page;

    private void Awake()
    {
        Instantiate(page, canvas.transform);
    }
}