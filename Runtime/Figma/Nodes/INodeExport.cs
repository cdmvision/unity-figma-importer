using System.Collections.Generic;

namespace Cdm.Figma
{
    public interface INodeExport
    {
        /// <summary>
        /// A list of export settings representing images to export from the canvas.
        /// </summary>
        public List<ExportSetting> exportSettings { get; set; }
    }
}