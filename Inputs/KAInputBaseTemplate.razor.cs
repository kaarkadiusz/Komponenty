using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Komponenty.Inputs
{
    public partial class KAInputBaseTemplate : KAComponentBase, IKAInputComponent
    {
        [Parameter]
        public string? Label { get; set; }
        [Parameter]
        public string? HelperText { get; set; }
        [Parameter]
        public AdornmentPosition? AdornmentPosition { get; set; }
        [Parameter]
        public string? AdornmentIcon { get; set; }
        [Parameter]
        public string? AdornmentText { get; set; }
        [Parameter]
        public InputType Type { get; set; }

        [Parameter, EditorRequired]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public Func<Task>? FocusInputTask { get; set; }
        [Parameter]
        public string? HtmlId { get; set; }
        [Parameter]
        public ValidationResult? ValidationResult { get; set; }
        [Parameter]
        public bool ReadOnly { get; set; }
        [Parameter]
        public bool Disabled { get; set; }

        private async Task FocusInput()
        {
            if (FocusInputTask is not null)
            {
                await FocusInputTask();
            }
        }

        protected override string GetCssClass()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("kompInputBaseTemplate");
            sb.AppendLine($"type{Type}");
            if (ValidationResult is not null)
            {
                sb.AppendLine($"validation{ValidationResult.Status}");
                switch (ValidationResult.Status)
                {
                    case ValidationStatus.Success:
                    case ValidationStatus.Warning:
                        sb.AppendLine("valid");
                        break;
                    case ValidationStatus.Error:
                        sb.AppendLine("invalid");
                        break;
                    case ValidationStatus.Initial:
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(ValidationResult.Message))
                {
                    sb.AppendLine("hasValidationMessage");
                }
            }
            sb.AppendLine(base.GetCssClass());

            return sb.ToString();
        }

        private string? GetHelperText()
        {
            if (!string.IsNullOrEmpty(ValidationResult?.Message))
            {
                return ValidationResult.Message;
            }
            return HelperText;
        }
    }
}
