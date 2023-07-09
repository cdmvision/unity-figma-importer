using Cdm.Figma.UI;
using UnityEngine;

[FigmaNode("MyCuteRectangleFrame")]
public class MyCuteRectangleFrame : MonoBehaviour
{
    [FigmaNode("MyCuteRectangle")]
    public GameObject rect;

    [FigmaResource("Cube")]
    public GameObject myCube;
    
    [FigmaAsset]
    public GameObject rectDependency;
    
    [FigmaAsset(name = "My Rectangle")]
    public GameObject rectDependencyWithName;
}