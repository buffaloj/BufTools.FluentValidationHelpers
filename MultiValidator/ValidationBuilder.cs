using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentValidation.Extensions
{
    public class ValidationBuilder
    {
        private readonly IList<Context> _validations;

        internal ValidationBuilder(IList<Context> validations)
        {
            _validations = validations ?? throw new ArgumentNullException(nameof(validations));
        }

        public ValidationBuilder With<TValidator, TObj>(TObj toValidate)
            where TValidator : IValidator<TObj>, new()
        {
            var validator = new TValidator();
            _validations.Add(new Context(validator, new ValidationContext<TObj>(toValidate)));

            return this;
        }

        public async Task ValidateAsync()
        {
            var errors = await GetValidationErrorsAsync();
            if (errors.Any())
                throw new ValidationException(errors);
        }

        public async Task<IEnumerable<ValidationFailure>> GetValidationErrorsAsync()
        {
            var tasks = _validations.Select(v => v.Validator.ValidateAsync(v.ValidationContext));
            var results = await Task.WhenAll(tasks);

            return results.Errors();
        }
    }
}
