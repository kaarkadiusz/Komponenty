using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Numerics;

namespace Komponenty.Inputs
{
    public partial class KAInputNumber<T> : KAInputBase<T>
    {
        [Parameter]
        public T? Min { get; set; }
        private bool _isMinParameterSet;
        [Parameter]
        public T? Max { get; set; }
        private bool _isMaxParameterSet;
        [Parameter]
        public T? Step { get; set; }
        private bool _isStepParameterSet;
        [Parameter]
        public bool UseSpinButton { get; set; }

        public override Type[] ValidTypes { get; } = [
            typeof(short), typeof(short?),
            typeof(int), typeof(int?),
            typeof(long), typeof(long?),
            typeof(float), typeof(float?),
            typeof(double), typeof(double?),
            typeof(decimal), typeof(decimal?),
            ];

        private Type BaseTypeParam => Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        private TypeWrapperBase NumberTypeWrapper { get; set; } = null!;
        private ValueTypeEnum ValueType { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitValueType();
            await InitTypeWrapper();
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            bool isParameterSet(string nameOfParam) => parameters.TryGetValue(nameOfParam, out T? _);
            _isMinParameterSet = isParameterSet(nameof(Min));
            _isMaxParameterSet = isParameterSet(nameof(Max));
            _isStepParameterSet = isParameterSet(nameof(Step));

            return base.SetParametersAsync(parameters);
        }

        private Task InitValueType()
        {
            ValueType = true switch
            {
                true when BaseTypeParam == typeof(short) => ValueTypeEnum.Short,
                true when BaseTypeParam == typeof(int) => ValueTypeEnum.Int,
                true when BaseTypeParam == typeof(long) => ValueTypeEnum.Long,
                true when BaseTypeParam == typeof(float) => ValueTypeEnum.Float,
                true when BaseTypeParam == typeof(double) => ValueTypeEnum.Double,
                true when BaseTypeParam == typeof(decimal) => ValueTypeEnum.Decimal,
                _ => throw new NotImplementedException()
            };
            return Task.CompletedTask;
        }
        private Task InitTypeWrapper()
        {
            Func<TValue?> getValue<TValue>() where TValue : struct, INumber<TValue> => () => (TValue?)(object?)Value;
            Func<TValue?, Task> setValue<TValue>() where TValue : struct, INumber<TValue> => async (value) => await InvokeValueChanged(value);
            Func<TValue?> getMin<TValue>() where TValue : struct, INumber<TValue> => () => !_isMinParameterSet || Min is null ? null : (TValue?)(object?)Min;
            Func<TValue?> getMax<TValue>() where TValue : struct, INumber<TValue> => () => !_isMaxParameterSet || Max is null ? null : (TValue?)(object?)Max;
            Func<TValue?> getStep<TValue>() where TValue : struct, INumber<TValue> => () => !_isStepParameterSet || Step is null ? null : (TValue?)(object?)Step;

            TypeWrapper<TValue> getTypeWrapper<TValue>() where TValue : struct, INumber<TValue>, IMinMaxValue<TValue> => new(getValue<TValue>(), setValue<TValue>(), getMin<TValue>(), getMax<TValue>(), getStep<TValue>());

            NumberTypeWrapper = ValueType switch
            {
                ValueTypeEnum.Short => getTypeWrapper<short>(),
                ValueTypeEnum.Int => getTypeWrapper<int>(),
                ValueTypeEnum.Long => getTypeWrapper<long>(),
                ValueTypeEnum.Float => getTypeWrapper<float>(),
                ValueTypeEnum.Double => getTypeWrapper<double>(),
                ValueTypeEnum.Decimal => getTypeWrapper<decimal>(),
                _ => throw new NotImplementedException(),
            };
            return Task.CompletedTask;
        }

        private CancellationTokenSource _pointerDownHandlerCTS { get; set; } = new();
        private const int _pointerDownInvokeDelay = 640;
        private const int _pointerDownInvokeMinDelay = 40;
        private async Task PointerDownHandler(Func<Task> task)
        {
            _pointerDownHandlerCTS.Cancel();
            _pointerDownHandlerCTS = new();
            int delay = _pointerDownInvokeDelay;
            CancellationToken token = _pointerDownHandlerCTS.Token;
            await Task.Delay(delay, token);
            while(!token.IsCancellationRequested)
            {
                await task();
                delay = Math.Max(delay / 2, _pointerDownInvokeMinDelay);
                await Task.Delay(delay, token);
            }
        }
        private async Task PointerUpHandler()
        {
            _pointerDownHandlerCTS.Cancel();
        }

        private abstract class TypeWrapperBase
        {
            public abstract T? GetValue();
            public abstract Task SetValue(T? value);
            public abstract object? GetMin();
            public abstract object? GetMax();
            public abstract object? GetStep();
            public abstract Task IncrementValueByStep();
            public abstract Task DecrementValueByStep();
        }

        private class TypeWrapper<TValue> : TypeWrapperBase where TValue : struct, INumber<TValue>, IMinMaxValue<TValue>
        {
            private readonly Func<TValue?> _valueFunc;
            private readonly Func<TValue?, Task> _changeValueFunc;
            private readonly Func<TValue?> _minFunc;
            private readonly Func<TValue?> _maxFunc;
            private readonly Func<TValue?> _stepFunc;

            public TypeWrapper(Func<TValue?> valueFunc, Func<TValue?, Task> changeValueFunc, Func<TValue?> minFunc, Func<TValue?> maxFunc, Func<TValue?> stepFunc)
            {
                _valueFunc = valueFunc;
                _changeValueFunc = changeValueFunc;
                _minFunc = minFunc;
                _maxFunc = maxFunc;
                _stepFunc = stepFunc;
            }

            private TValue? Value => _valueFunc();
            private TValue? Min => _minFunc();
            private TValue? Max => _maxFunc();
            private TValue? Step => _stepFunc();

            private static TValue DefaultStep
            {
                get
                {
                    if (TValue.IsInteger(TValue.One / (TValue.One + TValue.One)))
                    {
                        return TValue.One;
                    }
                    else
                    {
                        return TValue.CreateChecked(0.1);
                    }
                }
            }

            public override T? GetValue() => Value is null ? default : (T)(object)Value;
            public override async Task SetValue(T? value) => await SetValueInternal(value is null ? null : (TValue)Convert.ChangeType(value, typeof(TValue)));
            public override object? GetMin() => Min;
            public override object? GetMax() => Max;
            public override object? GetStep() => Step;
            public override async Task IncrementValueByStep() => await AddAndAlignToStep(Step ?? DefaultStep);
            public override async Task DecrementValueByStep() => await AddAndAlignToStep(-Step ?? -DefaultStep);

            private async Task AddAndAlignToStep(TValue amount)
            {
                TValue current = Value ?? Min ?? TValue.Zero;
                TValue remainder = (current - (Min ?? TValue.Zero)) % amount;
                if(remainder >= (Step ?? DefaultStep))
                {
                    if(amount > TValue.Zero)
                    {
                        amount -= remainder;
                    }
                    else if (remainder != TValue.Zero)
                    {
                        amount = -remainder;
                    }
                }
                TValue saturatedResult = TValue.CreateSaturating(current + amount);
                TValue finalResult = TValue.Clamp(saturatedResult, Min ?? TValue.MinValue, Max ?? TValue.MaxValue);
                await SetValueInternal(finalResult);
            }
            private async Task SetValueInternal(TValue? value) => await _changeValueFunc(value);
        }

        private enum ValueTypeEnum
        {
            Short,
            Int,
            Long,
            Float,
            Double,
            Decimal,
        }
    }
} 
