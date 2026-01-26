using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Inputs
{
    public class ValidationResult
    {
        public object? Key { get; set; }
        public ValidationStatus Status { get; set; }
        public string? Message { get; set; }
    }

    public enum ValidationStatus
    {
        Initial,
        Success,
        Warning,
        Error,
    }
}
