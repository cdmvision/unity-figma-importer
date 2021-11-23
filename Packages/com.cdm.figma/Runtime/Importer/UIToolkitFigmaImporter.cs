using System.Threading.Tasks;
using UnityEngine;

namespace Cdm.Figma
{
    [CreateAssetMenu(fileName = nameof(UIToolkitFigmaImporter), 
        menuName = AssetMenuRoot + "UI Toolkit Figma Importer", order = 1)]
    public class UIToolkitFigmaImporter : FigmaImporter
    {
        public override Task ImportFileAsync(FigmaFile file)
        {
            Debug.Log("TODO: IMPORT_FILE_ASYNC");
            return Task.CompletedTask;
        }
    }
}