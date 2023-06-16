using Cdm.Figma.UI;
using UnityEngine;
using UnityEngine.EventSystems;

[FigmaNode("PageBinding")]
public class PageBinding : UIBehaviour
{
    [FigmaNode("MyRect", isRequired = true)]
    [SerializeField]
    private GameObject _myRect;
}