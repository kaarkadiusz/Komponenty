using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Komponenty.Inputs
{
    public class KAValidation<T>
    {
        private readonly object _validationFunc;

        public KAValidation(Func<T?, string?> validationFunc)
        {
            _validationFunc = validationFunc;
        }
        public KAValidation(Func<T?, Task<string?>> validationFunc)
        {
            _validationFunc = validationFunc;
        }
        public KAValidation(Func<T?, IEnumerable<string>?> validationFunc)
        {
            _validationFunc = validationFunc;
        }
        public KAValidation(Func<T?, Task<IEnumerable<string>?>> validationFunc)
        {
            _validationFunc = validationFunc;
        }
        public KAValidation(Func<T?, KAValidationResult?> validationFunc)
        {
            _validationFunc = validationFunc;
        }
        public KAValidation(Func<T?, Task<KAValidationResult?>> validationFunc)
        {
            _validationFunc = validationFunc;
        }
        public KAValidation(Func<T?, IEnumerable<KAValidationResult>?> validationFunc)
        {
            _validationFunc = validationFunc;
        }
        public KAValidation(Func<T?, Task<IEnumerable<KAValidationResult>?>> validationFunc)
        {
            _validationFunc = validationFunc;
        }

        public async Task<IEnumerable<KAValidationResult>?> ValidateAsync(T? value)
        {
            IEnumerable<KAValidationResult>? results = null;
            switch (_validationFunc)
            {
                case Func<T?, string?> func:
                    {
                        string? funcResult = func(value);
                        results = funcResult is not null ? [new KAValidationResult(funcResult)] : null;
                    }
                    break;
                case Func<T?, Task<string?>> func:
                    {
                        string? funcResult = await func(value);
                        results = funcResult is not null ? [new KAValidationResult(funcResult)] : null;
                    }
                    break;
                case Func<T?, IEnumerable<string>?> func:
                    {
                        IEnumerable<string>? funcResult = func(value);
                        results = funcResult?.Select(result => new KAValidationResult(result));
                    }
                    break;
                case Func<T?, Task<IEnumerable<string>?>> func:
                    {
                        IEnumerable<string>? funcResult = await func(value);
                        results = funcResult?.Select(result => new KAValidationResult(result));
                    }
                    break;
                case Func<T?, KAValidationResult?> func:
                    {
                        KAValidationResult? funcResult = func(value);
                        results = funcResult is not null ? [funcResult] : null;
                    }
                    break;
                case Func<T?, Task<KAValidationResult?>> func:
                    {
                        KAValidationResult? funcResult = await func(value);
                        results = funcResult is not null ? [funcResult] : null;
                    }
                    break;
                case Func<T?, IEnumerable<KAValidationResult>?> func:
                    {
                        IEnumerable<KAValidationResult>? funcResult = func(value);
                        results = funcResult;
                    }
                    break;
                case Func<T?, Task<IEnumerable<KAValidationResult>?>> func:
                    {
                        IEnumerable<KAValidationResult>? funcResult = await func(value);
                        results = funcResult;
                    }
                    break;

            }

            return results;
        }
    }
}
