using Komponenty.Classes;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Komponenty.Basic
{
    public partial class KAToggle<T> : KAComponentBase
    {
        [Parameter]
        public T? Value { get; set; }
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }
        [Parameter]
        public T?[]? ValueTable { get; set; }
        [Parameter]
        public KAToggleSize Size { get; set; } = KAToggleSize.Medium;
        [Parameter]
        public Color Color { get; set; } = Color.Primary;
        [Parameter]
        public Func<T?, Color>? ColorFunc { get; set; }
        [Parameter]
        public Func<T?, string>? IconFunc { get; set; }
        [Parameter]
        public RenderFragment<T?>? IconFragment { get; set; }
        [Parameter]
        public bool DisplayVertical { get; set; }

        private T?[] _defaultValues = [];

        protected override async Task OnInitializedAsync()
        {
            await SetDefaultValues();
            await base.OnInitializedAsync();
        }

        private T?[] Values => ValueTable ?? _defaultValues;

        private async Task SetDefaultValues()
        {
            _defaultValues = [];
            if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
            {
                _defaultValues = [(T)(object)false, (T)(object)(true)];
            }
            else if (typeof(T).IsEnum)
            {
                _defaultValues = [.. (T[])Enum.GetValues(typeof(T))];
            }
        }

        protected override string GetCssClass()
        {
            StringBuilder sb = new();

            sb.AppendLine("ka-toggle");
            sb.AppendLine($"ka-color-{GetColor()}");
            sb.AppendLine($"ka-size-{Size}");
            if (DisplayVertical)
            {
                sb.AppendLine("ka-vertical");
            }

            sb.AppendLine(base.GetCssClass());

            return sb.ToString();
        }
        private Color GetColor()
        {
            if (ColorFunc is not null)
            {
                return ColorFunc(Value);
            }
            return Equals(Value, Values.LastOrDefault()) ? Color : Color.Default;
        }
        private string GetStyleString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"--indicator-index: {Values.IndexOf(Value!)}");

            return sb.ToString();
        }

        private async Task OptionClicked(T? value)
        {
            if(Equals(value, Value))
            {
                await SetNextValue();
            }
            else
            {
                await SetValue(value);
            }
        }
        private async Task SetNextValue()
        {
            int index = Values.IndexOf(Value!);
            if (index == -1)
            {
                return;
            }

            await SetValue(Values[(index + 1) % Values.Length]);
        }
        private async Task SetValue(T? value)
        {
            Value = value;
            await ValueChanged.InvokeAsync(Value);
        }
    }

    public enum KAToggleSize
    {
        Small,
        Medium,
        Large
    }
}
