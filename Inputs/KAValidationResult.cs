using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Komponenty.Inputs
{
    public class KAValidationResult
    {
        public KAValidationResult()
        {
        }
        public KAValidationResult(string message)
        {
            Message = message;
        }
        public KAValidationResult(string message, KAValidationStatus status)
        {
            Message = message;
            Status = status;
        }

        public string? Message { get; set; }
        public KAValidationStatus Status { get; set; } = KAValidationStatus.Error;
    }

    public enum KAValidationStatus
    {
        Success,
        Warning,
        Error,
    }
}
