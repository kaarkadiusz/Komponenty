using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Komponenty.Inputs
{
    public abstract class KAInputBase<T> : InputBase<T>, IKAInputComponent
    {
        [Parameter]
        public virtual string? Label { get; set; }
        [Parameter]
        public virtual string? HelperText { get; set; }
        [Parameter]
        public virtual bool Disabled { get; set; }
        [Parameter]
        public virtual bool ReadOnly { get; set; }
        [Parameter]
        public virtual AdornmentPosition? AdornmentPosition { get; set; }
        [Parameter]
        public virtual string? AdornmentIcon { get; set; }
        [Parameter]
        public virtual string? AdornmentText { get; set; }
        [Parameter]
        public virtual InputType Type { get; set; } = InputType.Filled;

        [Parameter]
        public virtual ValueEvent ValueEvent { get; set; } = ValueEvent.OnChange;
        [Parameter]
        public virtual Func<Task<ValidationResult?>>? ValidateTask { get; set; }
        [Parameter]
        public virtual ValidationBehavior ValidationBehavior { get; set; } = ValidationBehavior.OnValueChange;
        [Parameter]
        public bool DisableDataAnnotationsValidation { get; set; }

        protected ElementReference? ElementReference { get; set; }
        protected string IdAttribute => $"{GetType().Name}-{GetHashCode()}";

        public virtual Type[] ValidTypes { get; } = [];

        protected string BoundEvent
        {
            get
            {
                return ValueEvent switch
                {
                    ValueEvent.OnInput => "oninput",
                    _ or ValueEvent.OnChange => "onchange",
                };
            }
        }

        public ValidationResult? RecentValidationResult { get; protected set; }

        protected KAInputBase()
        {
            ValueExpression ??= () => Value!;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (ValidTypes.Length > 0)
            {
                Type providedType = typeof(T);
                if (!ValidTypes.Contains(providedType))
                {
                    throw new InvalidOperationException($"The provided generic type {providedType.Name} is not a valid type for {GetType().Name}.\nValid types include: {string.Join(", ", ValidTypes)}");
                }
            }
        }

        protected override void OnParametersSet()
        {
            ValueExpression ??= () => Value!;
            base.OnParametersSet();
        }

        protected override bool TryParseValueFromString(string? value, out T result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            validationErrorMessage = default;
            string validationDefaultError = "There was an error parsing value.";

            if (value is null)
            {
                result = default!;
                return true;
            }
            try
            {
                result = (T)(object)value;
            }
            catch
            {
                validationErrorMessage = validationDefaultError;
                result = default!;
                return false;
            }
            return true;
        }

        protected virtual string GetCssClass()
        {
            StringBuilder sb = new();

            if (AdditionalAttributes?.TryGetValue("class", out object? classAttribute) ?? false)
            {
                sb.AppendLine(classAttribute.ToString());
            }

            return sb.ToString();
        }


        public virtual async Task Validate()
        {
            RecentValidationResult = null;
            if (ValidateTask is not null)
            {
                RecentValidationResult = await ValidateTask.Invoke();
            }
            RecentValidationResult ??= (DisableDataAnnotationsValidation ? null : ValidateDataAnnotations());
            await InvokeAsync(StateHasChanged);
        }
        private ValidationResult? ValidateDataAnnotations()
        {
            object? value = null;
            IEnumerable<ValidationAttribute> validationAttributes = [];
            if (FieldIdentifier.Model.GetType().GetProperty(FieldIdentifier.FieldName) is PropertyInfo propertyInfo && propertyInfo.GetGetMethod(nonPublic: false) is not null)
            {
                value = propertyInfo.GetValue(FieldIdentifier.Model);
                validationAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>();
            }
            else if (FieldIdentifier.Model.GetType().GetField(FieldIdentifier.FieldName) is FieldInfo fieldInfo && fieldInfo.IsPublic)
            {
                value = fieldInfo.GetValue(FieldIdentifier.Model);
                validationAttributes = fieldInfo.GetCustomAttributes<ValidationAttribute>();
            }

            ValidationContext context = new(FieldIdentifier.Model)
            {
                MemberName = FieldIdentifier.FieldName
            };
            ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults = [];
            Validator.TryValidateValue(value, context, validationResults, validationAttributes);
            if (validationResults.FirstOrDefault() is System.ComponentModel.DataAnnotations.ValidationResult validationResult)
            {
                return new ValidationResult() { Message = validationResult.ErrorMessage, Status = ValidationStatus.Error };
            }

            return null;
        }

        public virtual async Task FocusAsync()
        {
            if (ElementReference is not null)
            {
                await ElementReference.Value.FocusAsync();
            }
        }

        protected virtual async Task InvokeValueChanged(T? value)
        {
            await ValueChanged.InvokeAsync(value);
            await TryValidate();
        }
        protected async Task InvokeValueChanged(object? value)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }
            T? result = (T?)value ?? default;
            await InvokeValueChanged(result);
        }

        private const int ValidateDebounceDelay = 500;

        private CancellationTokenSource ValidateDebounceCTS { get; set; } = new();
        private async Task TryValidate()
        {
            switch (ValidationBehavior)
            {
                case ValidationBehavior.OnValueChange:
                    await Validate();
                    break;
                case ValidationBehavior.OnValueChangeWithDebouncing:
                    ValidateDebounceCTS.Cancel();
                    ValidateDebounceCTS = new();
                    CancellationToken token = ValidateDebounceCTS.Token;
                    await Task.Delay(ValidateDebounceDelay, token).ContinueWith(async task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            await Validate();
                        }
                    });
                    break;
                case ValidationBehavior.Manual:
                default:
                    break;
            }
        }

        private new object? DisplayName { get; }
    }

    public enum InputType
    {
        Filled,
        Outlined,
        Clear
    }
    public enum AdornmentPosition
    {
        Left = 1,
        Right = 2,
    }
    public enum ValidationBehavior
    {
        OnValueChange,
        OnValueChangeWithDebouncing,
        Manual
    }
    public enum ValueEvent
    {
        OnChange,
        OnInput
    }
}
