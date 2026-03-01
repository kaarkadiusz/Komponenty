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
        public string? Label { get; set; }
        [Parameter]
        public string? HelperText { get; set; }
        [Parameter]
        public bool Disabled { get; set; }
        [Parameter]
        public bool ReadOnly { get; set; }
        [Parameter]
        public AdornmentPosition? AdornmentPosition { get; set; }
        [Parameter]
        public string? AdornmentIcon { get; set; }
        [Parameter]
        public string? AdornmentText { get; set; }
        [Parameter]
        public InputVariant Variant { get; set; } = InputVariant.Filled;

        [Parameter]
        public ValueEvent ValueEvent { get; set; } = ValueEvent.OnChange;

        [Parameter]
        public KAValidation<T>? Validation { get; set; }
        [Parameter]
        public ValidationBehavior ValidationBehavior { get; set; } = ValidationBehavior.OnValueChange;

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

        public KAValidationResult? RecentValidationResult { get; private set; }
        public IReadOnlyList<KAValidationResult> ValidationResults => _validationResults;
        private readonly List<KAValidationResult> _validationResults = [];

        public IEnumerable<string>? GetValidationErrors() => EditContext?.GetValidationMessages(FieldIdentifier);
        private ValidationMessageStore? _validationMessageStore = null!;

        protected KAInputBase()
        {
            ValueExpression ??= () => Value!;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await ThrowIfInvalidType();

            if(EditContext is not null)
            {
                _validationMessageStore = new(EditContext);
                EditContext.OnValidationRequested += HandleValidationRequest;
            }
        }
        private async void HandleValidationRequest(object? sender, ValidationRequestedEventArgs args)
        {
            await ValidateAsync();
        }
        private Task ThrowIfInvalidType()
        {
            if (ValidTypes.Length > 0)
            {
                Type providedType = typeof(T);
                if (!ValidTypes.Contains(providedType))
                {
                    throw new InvalidOperationException($"The provided generic type {providedType.Name} is not a valid type for {GetType().Name}.\nValid types include: {string.Join(", ", ValidTypes)}");
                }
            }
            return Task.CompletedTask;
        }

        protected override void OnParametersSet()
        {
            ValueExpression ??= () => Value!;
            base.OnParametersSet();
        }

        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out T result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            throw new NotImplementedException();
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

        protected virtual async Task InvokeValueChanged(T? value)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }

            CurrentValue = value;

            await TryValidate();
        }
        protected async Task InvokeValueChanged(object? value)
        {
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
                    await ValidateAsync();
                    break;
                case ValidationBehavior.OnValueChangeWithDebouncing:
                    ValidateDebounceCTS.Cancel();
                    ValidateDebounceCTS = new();
                    CancellationToken token = ValidateDebounceCTS.Token;
                    await Task.Delay(ValidateDebounceDelay, token).ContinueWith(async task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            await ValidateAsync();
                        }
                    });
                    break;
                case ValidationBehavior.Manual:
                default:
                    break;
            }
        }

        public virtual async Task ValidateAsync()
        {
            _validationMessageStore?.Clear();
            _validationResults.Clear();
            EditContext?.NotifyFieldChanged(FieldIdentifier);
            if (Validation is not null)
            {
                IEnumerable<KAValidationResult>? validationResults = await Validation.ValidateAsync(CurrentValue);
                if(validationResults is not null)
                {
                    _validationResults.AddRange(validationResults);
                    IEnumerable<KAValidationResult> validationErrors = validationResults.Where(validationResult => validationResult.Status is KAValidationStatus.Error);
                    if(validationErrors.Any() && _validationMessageStore is not null)
                    {
                        foreach(KAValidationResult validationResult in validationErrors)
                        {
                            _validationMessageStore.Add(FieldIdentifier, validationResult.Message ?? string.Empty);
                        }
                        EditContext?.NotifyValidationStateChanged();
                    }
                }
            }

            await RefreshRecentValidationResult();

            await InvokeAsync(StateHasChanged);
        }

        private Task RefreshRecentValidationResult()
        {
            string? error = EditContext?.GetValidationMessages(FieldIdentifier).FirstOrDefault();
            if(error is not null)
            {
                RecentValidationResult = new KAValidationResult(error);
            }
            else
            {
                RecentValidationResult = ValidationResults.Count > 0 ? ValidationResults[0] : null;
            }
            return Task.CompletedTask;
        }

        public virtual async Task FocusAsync()
        {
            if (ElementReference is not null)
            {
                await ElementReference.Value.FocusAsync();
            }
        }

        private new object? DisplayName { get; }

        protected override void Dispose(bool disposing)
        {
            EditContext?.OnValidationRequested -= HandleValidationRequest;
            base.Dispose(disposing);
        }
    }

    public enum InputVariant
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
