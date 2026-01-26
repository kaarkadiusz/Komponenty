using Komponenty.Classes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Komponenty.Inputs
{
    public partial class KAInputRadio<T> : KAComponentBase
    {
        [CascadingParameter]
        public KAInputRadioGroup<T> RadioGroup { get; set; } = null!;
        [Parameter, EditorRequired]
        public T Value { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public Color Color { get; set; } = Color.Default;

        private string IdAttribute => $"{nameof(KAInputRadio<>)}-{GetHashCode()}";
        private bool IsSelected => RadioGroup.Value?.Equals(Value) ?? false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if(RadioGroup is null)
            {
                throw new InvalidOperationException($"There was no {nameof(KAInputRadioGroup<T>)} found to be associated with this {nameof(KAInputRadio<T>)}. \nThe {nameof(KAInputRadio<T>)} has to be rendered as a child of {nameof(KAInputRadioGroup<T>)}.");
            }
            await RadioGroup.AddStateHasChangedTask(async () => await InvokeAsync(StateHasChanged));
        }

        protected override string GetCssClass()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("kompInputRadio");

            sb.AppendLine($"color-{Color}");

            sb.AppendLine(base.GetCssClass());
            return sb.ToString();
        }

        private async Task InvokeValueChanged()
        {
            await RadioGroup.SetValue(Value);
        }
    }
}
