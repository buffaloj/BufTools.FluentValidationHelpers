using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentValidation.Extensions
{
    public class ValidationBuilder
    {
        private readonly IList<Context> _validations = new List<Context>();
        private readonly IValidatorFactory _validatorFactory;

        internal ValidationBuilder(Context context, IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
            _validations.Add(context);
        }

        public ContextBuilder<TObj> Check<TObj>(TObj toValidate)
            where TObj : class
        {
            var context = new Context(new ValidationContext<TObj>(toValidate), false);
            _validations.Add(context);
            return new ContextBuilder<TObj>(context, this);
        }

        public ContextBuilder<TObj> CheckOptional<TObj>(TObj? toValidate)
           where TObj : class
        {
            var context = new Context((toValidate != null) ? new ValidationContext<TObj>(toValidate) : null, true);
            _validations.Add(context);
            return new ContextBuilder<TObj>(context, this);
        }

        public async Task ValidateAsync()
        {
            var errors = await GetValidationErrorsAsync();
            if (errors.Any())
                throw new ValidationException(errors);
        }

        public async Task<IEnumerable<ValidationFailure>> GetValidationErrorsAsync()
        {
            if (_validations.Any(v => v.ValidatorType == null))
                throw new ArgumentNullException(nameof(Context.ValidatorType));

            var tasks = _validations.Where(v => !v.IsOptional || (v.IsOptional && v.ValidationContext != null))
                                    .Select(v => _validatorFactory.GetValidator(v.ValidatorType)?.ValidateAsync(v.ValidationContext));
            var results = await Task.WhenAll(tasks);

            return results.Errors();
        }
    }
}
