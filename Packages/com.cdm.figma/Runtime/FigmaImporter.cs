using System.Threading.Tasks;
using UnityEngine;

namespace Cdm.Figma
{
    public abstract class FigmaImporter : ScriptableObject, IFigmaImporter
    {
        public const string AssetMenuRoot = "Cdm/Figma/";

        public abstract FigmaFile CreateFile(string fileId, string fileContent, byte[] thumbnailData = null);
        public abstract Task ImportFileAsync(FigmaFile file);
    }
}