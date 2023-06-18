using Cdm.Figma.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

[FigmaNode("PageBinding")]
public class PageBinding : UIBehaviour
{
    [FigmaNode("MyRect", isRequired = true)]
    [SerializeField]
    private GameObject _myRect;

    [FigmaLocalize("My Table/myText")]
    [SerializeField]
    private LocalizedString _myLocalizedString;
}