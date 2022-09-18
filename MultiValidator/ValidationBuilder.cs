using FluentValidation.Extensions.Exceptions;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentValidation.Extensions
{
    /// <summary>
    /// A class used to build up sets of validations to perform as a group
    /// </summary>
    public class ValidationBuilder
    {
        private readonly IList<Context> _validations = new List<Context>();
        private readonly IValidatorFactory _validatorFactory;

        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="context">An initial context to start with</param>
        /// <param name="validatorFactory">A factory that fetches instances of validators </param>
        /// <exception cref="ArgumentNullException">Thrown when a param is null</exception>
        internal ValidationBuilder(Context context, IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
            _validations.Add(context);
        }

        /// <summary>
        /// Adds an object to the validation
        /// </summary>
        /// <typeparam name="TObj">The type of the object to validate</typeparam>
        /// <param name="toValidate">The object to validate</param>
        /// <returns>A <see cref="ContextBuilder{TObj}"/> instance so more info can be provided for the validation</returns>
        public ContextBuilder<TObj> Check<TObj>(TObj toValidate)
            where TObj : class
        {
            var context = new Context(new ValidationContext<TObj>(toValidate), false);
            _validations.Add(context);
            return new ContextBuilder<TObj>(context, this);
        }

        /// <summary>
        /// Adds an optional object to the validation - the validation will pass when the object is null
        /// </summary>
        /// <typeparam name="TObj">The type of the object to validate</typeparam>
        /// <param name="toValidate">The object to validate</param>
        /// <returns>A <see cref="ContextBuilder{TObj}"/> instance so more info can be provided for the validation</returns>
        public ContextBuilder<TObj> CheckOptional<TObj>(TObj? toValidate)
           where TObj : class
        {
            var context = new Context((toValidate != null) ? new ValidationContext<TObj>(toValidate) : null, true);
            _validations.Add(context);
            return new ContextBuilder<TObj>(context, this);
        }

        /// <summary>
        /// Asynchronously performs validation on all objects in this set and throws an exception with the combined error lists
        /// </summary>
        /// <returns>A <see cref="Task"/> to await</returns>
        /// <exception cref="ValidationException">Thrown if any of the objects have a validation error</exception>
        public async Task ValidateAsync()
        {
            var errors = await GetValidationErrorsAsync();
            if (errors.Any())
                throw new ValidationException(errors);
        }

        /// <summary>
        /// Asynchronously performs validation on all objects in this set and returns any errors found
        /// </summary>
        /// <returns>A collection of error or an empty one if there are no errors.</returns>
        /// <exception cref="ValidatorTypeNotFoundException">Thrown if any of ther validators were not found</exception>
        public async Task<IEnumerable<ValidationFailure>> GetValidationErrorsAsync()
        {
            if (_validations.Any(v => v.ValidatorType == null))
                throw new ValidatorTypeNotFoundException(nameof(Context.ValidatorType));

            var tasks = _validations.Where(v => !v.IsOptional || (v.IsOptional && v.ValidationContext != null))
                                    .Select(v => _validatorFactory.GetValidator(v.ValidatorType).ValidateAsync(v.ValidationContext));
            var results = await Task.WhenAll(tasks);

            return results.Errors();
        }

        /// <summary>
        /// Performs validation on all objects in this set and throws an exception with the combined error lists
        /// </summary>
        /// <exception cref="ValidationException">Thrown if any of the objects have a validation error</exception>
        public void Validate()
        {
            var errors = GetValidationErrors();
            if (errors.Any())
                throw new ValidationException(errors);
        }

        /// <summary>
        /// Synchronously performs validation on all objects in this set and returns any errors found
        /// </summary>
        /// <returns>A collection of error or an empty one if there are no errors.</returns>
        /// <exception cref="ValidatorTypeNotFoundException">Thrown if any of ther validators were not found</exception>
        public IEnumerable<ValidationFailure> GetValidationErrors()
        {
            if (_validations.Any(v => v.ValidatorType == null))
                throw new ValidatorTypeNotFoundException(nameof(Context.ValidatorType));

            var results = _validations.Where(v => !v.IsOptional || (v.IsOptional && v.ValidationContext != null))
                                      .Select(v => _validatorFactory.GetValidator(v.ValidatorType).Validate(v.ValidationContext)).ToArray();

            return results.Errors();
        }
    }
}
