using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(Figma.FigmaTaskFile), menuName = AssetMenuRoot + "Figma Task File", order = 0)]
    public class FigmaTaskFile : FigmaTaskFile<FigmaImporter, FigmaDownloader>
    {
        protected new const string AssetMenuRoot = 
            FigmaTaskFile<FigmaImporter, FigmaDownloader>.AssetMenuRoot + "UIToolkit/";
    }
}