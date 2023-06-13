using System;

namespace Cdm.Figma.Utils
{
    public class SvgImportException : Exception
    {
        public string svg { get; }

        public SvgImportException()
        {
        }

        public SvgImportException(string message, string svg) : base(message)
        {
            this.svg = svg;
        }

        public SvgImportException(string message, string svg, Exception innerException) :
            base(message, innerException)
        {
            this.svg = svg;
        }
    }
}