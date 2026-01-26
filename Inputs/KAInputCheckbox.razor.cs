using Komponenty.Classes;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Komponenty.Inputs
{
    public partial class KAInputCheckbox<T> : KAInputBase<T>
    {
        [Parameter]
        public string CheckedIcon { get; set; } = Icons.Preset.Fill.SquareCheck;
        [Parameter]
        public string UncheckedIcon { get; set; } = Icons.Preset.Outline.Square;
        [Parameter]
        public string UnknownIcon { get; set; } = Icons.Preset.Outline.HelpSquare;

        [Parameter]
        public Color Color { get; set; } = Color.Default;

        public override Type[] ValidTypes => [
            typeof(bool), typeof(bool?)
            ];

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }

        protected override string GetCssClass()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("kompInputCheckboxWrap");

            sb.AppendLine($"color-{Color}");

            switch (RecentValidationResult?.Status)
            {
                case ValidationStatus.Initial:
                case null:
                    break;
                default:
                    sb.AppendLine($"validation-{RecentValidationResult.Status}");
                    break;
            }

            sb.AppendLine(base.GetCssClass());
            return sb.ToString();
        }

        private async Task InvokeValueChanged()
        {
            switch (Value)
            {
                case true:
                    await InvokeValueChanged(false);
                    break;
                case false:
                    if (Nullable.GetUnderlyingType(typeof(T)) is not null)
                    {
                        await InvokeValueChanged(null);
                    }
                    else
                    {
                        await InvokeValueChanged(true);
                    }
                    break;
                case null:
                    await InvokeValueChanged(true);
                    break;
            }
        }

        private string? GetHelperText()
        {
            if (!string.IsNullOrEmpty(RecentValidationResult?.Message))
            {
                return RecentValidationResult.Message;
            }
            return HelperText;
        }

        private new object? ValueEvent { get; }
        private new object? AdornmentIcon { get; }
        private new object? AdornmentPosition { get; }
        private new object? AdornmentText { get; }
        private new object? Type { get; }
        private new object? ReadOnly { get; }
    }
}
