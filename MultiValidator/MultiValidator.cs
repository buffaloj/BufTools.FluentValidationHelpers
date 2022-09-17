using FluentValidation;
using FluentValidation.Results;
using System;
using System.Threading.Tasks;

namespace MultiValidator
{
    public class MultiValidator
    {
        private object? _toValidate;
        private IValidator? _validator; 

        public MultiValidator With<TValidator>(object toValidate)
            where TValidator : IValidator, new()
        {
            _toValidate = toValidate ?? throw new ArgumentNullException(nameof(toValidate));
            _validator = new TValidator();

            return this;
        }

        public async Task ValidateAsync()
        {
            var result = await GetValidationResultAsync();
            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }

        public async Task<ValidationResult> GetValidationResultAsync()
        {
            if (_validator == null || _toValidate == null)
                return new ValidationResult();

            return await _validator.ValidateAsync(new ValidationContext<object>(_toValidate));
        }
    }
}
