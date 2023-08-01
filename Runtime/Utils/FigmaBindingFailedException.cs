using System;
using System.Collections.Generic;

namespace Cdm.Figma.UI.Utils
{
    public class FigmaBindingFailedException : Exception
    {
        public IList<BindingError> errors { get; }

        public FigmaBindingFailedException(IList<BindingError> errors)
            : base("Binding of some figma nodes failed.")
        {
            this.errors = errors;
        }

        public FigmaBindingFailedException(IList<BindingError> errors, string message)
            : base(message)
        {
            this.errors = errors;
        }

        public FigmaBindingFailedException(IList<BindingError> errors, string message, Exception inner)
            : base(message, inner)
        {
            this.errors = errors;
        }
    }
}