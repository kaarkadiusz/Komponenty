using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Komponenty.Inputs
{
    public partial class KAInputRadioGroup<T> : KAInputBase<T>
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public RadioGroupDirection Direction { get; set; } = RadioGroupDirection.Row;

        private List<Func<Task>> _stateHasChangedTasks = [];
        private T? _value;

        internal async Task SetValue(T? value)
        {
            await InvokeValueChanged(value);
            await InvokeAsync(StateHasChanged);
        }
        internal Task AddStateHasChangedTask(Func<Task> stateHasChangedTask)
        {
            _stateHasChangedTasks.Add(stateHasChangedTask);
            return Task.CompletedTask;
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if(_value is null || !_value.Equals(Value))
            {
                _value = Value;
                await RunStateHasChangedTasks();
            }
        }

        private async Task RunStateHasChangedTasks()
        {
            IEnumerable<Task> tasks = _stateHasChangedTasks.Select(task => task()).Where(task => task is not null);
            await Task.WhenAll(tasks);
        }

        private string? GetHelperText()
        {
            if (!string.IsNullOrEmpty(RecentValidationResult?.Message))
            {
                return RecentValidationResult.Message;
            }
            return HelperText;
        }

        protected override string GetCssClass()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("kompInputRadioGroupWrap");
            sb.AppendLine($"direction-{Direction}");

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

        private new object? Label { get; }
        private new object? ReadOnly { get; }
        private new object? ValueEvent { get; }
        private new object? AdornmentIcon { get; }
        private new object? AdornmentPosition { get; }
        private new object? AdornmentText { get; }
        private new object? Type { get; }
    }

    public enum RadioGroupDirection
    {
        Row,
        Column
    }
}
