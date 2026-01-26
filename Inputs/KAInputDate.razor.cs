using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace Komponenty.Inputs
{
    public partial class KAInputDate<T> : KAInputBase<T>
    {
        [Parameter]
        public T? Min { get; set; }
        [Parameter]
        public T? Max { get; set; }

        public new ValueEvent ValueEvent => base.ValueEvent;

        public override Type[] ValidTypes { get; } = [
            typeof(DateTime), typeof(DateTime?),
            typeof(DateOnly), typeof(DateOnly?)
            ];

        private Type BaseTypeParam => Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        private ValueTypeEnum ValueType { get; set; }
        private InputDateGenericTypeWrapper<DateTime>? DateTimeTypeWrapper { get; set; }
        private InputDateGenericTypeWrapper<DateOnly>? DateOnlyTypeWrapper { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            base.ValueEvent = ValueEvent.OnInput;
            await EvaluateValueType();
            await InitGenericTypeWrappers();
        }
        private Task EvaluateValueType()
        {
            ValueType = true switch
            {
                true when BaseTypeParam == typeof(DateTime) => ValueTypeEnum.DateTime,
                true when BaseTypeParam == typeof(DateOnly) => ValueTypeEnum.DateOnly,
                _ => throw new NotImplementedException()
            };
            return Task.CompletedTask;
        }
        private Task InitGenericTypeWrappers()
        {
            switch (ValueType)
            {
                case ValueTypeEnum.DateTime:
                    DateTimeTypeWrapper = new(() => (DateTime?)(object?)Value, () => (DateTime?)(object?)Min, () => (DateTime?)(object?)Max);
                    break;
                case ValueTypeEnum.DateOnly:
                    DateOnlyTypeWrapper = new(() => (DateOnly?)(object?)Value, () => (DateOnly?)(object?)Min, () => (DateOnly?)(object?)Max);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return Task.CompletedTask;
        }

        private string GetBoundaryString(DateTime? boundary)
        {
            if (boundary is DateTime boundaryDt)
            {
                return boundaryDt.ToString("yyyy-MM-ddTHH:mm");
            }
            return string.Empty;
        }
        private string GetBoundaryString(DateOnly? boundary)
        {
            if (boundary is DateOnly boundaryDo)
            {
                return boundaryDo.ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }

        private class InputDateGenericTypeWrapper<TValue> where TValue : struct
        {
            private Func<TValue?> ValueExpression { get; set; }
            private Func<TValue?> MinExpression { get; set; }
            private Func<TValue?> MaxExpression { get; set; }

            private TValue? GetParameterValue(Func<TValue?> expression)
            {
                TValue? value = expression.Invoke();
                if (value is null || value.Equals(default(TValue)))
                {
                    return null;
                }
                return value;
            }

            public TValue? Value => ValueExpression.Invoke();
            public TValue? Min => GetParameterValue(MinExpression);
            public TValue? Max => GetParameterValue(MaxExpression);

            public InputDateGenericTypeWrapper(Func<TValue?> valueExpression, Func<TValue?> minExpression, Func<TValue?> maxExpression)
            {
                ValueExpression = valueExpression;
                MinExpression = minExpression;
                MaxExpression = maxExpression;
            }
        }

        private enum ValueTypeEnum
        {
            DateTime,
            DateOnly
        }
    }
}
