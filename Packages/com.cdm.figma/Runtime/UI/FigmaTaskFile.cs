using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(Figma.FigmaTaskFile), 
        menuName = FigmaImporter.AssetMenuRoot + "Figma Task File", order = 0)]
    public class FigmaTaskFile : FigmaTaskFile<FigmaImporter>
    {
    }
}