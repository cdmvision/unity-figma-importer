using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(Figma.FigmaTaskFile), menuName = AssetMenuRoot + "Figma Task File", order = 0)]
    public class FigmaTaskFile : FigmaTaskFile<FigmaImporter>
    {
        protected new const string AssetMenuRoot = FigmaTaskFile<FigmaImporter>.AssetMenuRoot + "UIToolkit/";
        
    }
}