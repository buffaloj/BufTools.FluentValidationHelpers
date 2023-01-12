using FluentValidation.Extensions.Exceptions;
using FluentValidation.Extensions.Extensions;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentValidation.Extensions
{
    /// <summary>
    /// Class that aggregates multiple validations together and performs them as a group.
    /// </summary>
    public class ValidationAggregator
    {
        private readonly IList<Validation> _validations = new List<Validation>();
        private readonly IValidatorFactory _validatorFactory;

        /// <summary>
        /// Constructs an object instance 
        /// </summary>
        /// <param name="factory">The factory to use to create validators</param>
        internal ValidationAggregator(IValidatorFactory factory) => _validatorFactory = factory;

        /// <summary>
        /// Adds a validation to the current group
        /// </summary>
        /// <param name="validation">The validation to add</param>
        /// <returns>A class to chain more validations</returns>
        internal ValidationAggregator AddValidation(Validation validation)
        {
            _validations.Add(validation);
            return this;
        }

        /// <summary>
        /// Supplies an instance of an object to validate
        /// </summary>
        /// <typeparam name="TObj">The Type of the object to validate</typeparam>
        /// <param name="instance">An instance to validate</param>
        /// <returns>A <see cref="ValidationBuilder{TObj}"/> instance to fluently supply the type of validator to use</returns>
        /// <exception cref="NullObjectException">Thrown if the instance is null</exception>
        /// <exception cref="ValidatorTypeNotFoundException">Thrown when the validator type is not found in the service provider</exception>
        public ValidationBuilder<TObj> For<TObj>(TObj instance)
            where TObj : class
        {
            return new ValidationBuilder<TObj>(this, instance, false);
        }

        /// <summary>
        /// Supplies an instance of an object to validate that will be skipped if null with no error returned
        /// </summary>
        /// <typeparam name="TObj">The Type of the object to validate</typeparam>
        /// <param name="instance">An instance to validate</param>
        /// <returns>A <see cref="ValidationBuilder{TObj}"/> instance to fluently supply the type of validator to use</returns>
        /// <exception cref="ValidatorTypeNotFoundException">Thrown when the validator type is not found in the service provider</exception>
        public ValidationBuilder<TObj> ForOptional<TObj>(TObj? instance)
            where TObj : class
        {
            return new ValidationBuilder<TObj>(this, instance, true);
        }

        /// <summary>
        /// Asynchronously performs validation on all objects in this set and throws an exception with the combined error lists
        /// </summary>
        /// <returns>A <see cref="Task"/> to await</returns>
        /// <exception cref="ValidationException">Thrown if any of the objects have a validation error</exception>
        public async Task ValidateAsync(CancellationToken cancellation = new CancellationToken())
        {
            var errors = await GetValidationErrorsAsync(cancellation);
            if (errors.Any())
                throw new ValidationException(errors);
        }

        /// <summary>
        /// Asynchronously performs validation on all objects in this set and returns any errors found
        /// </summary>
        /// <returns>A collection of error or an empty one if there are no errors.</returns>
        /// <exception cref="ValidatorTypeNotFoundException">Thrown if any of ther validators were not found</exception>
        public async Task<IEnumerable<ValidationFailure>> GetValidationErrorsAsync(CancellationToken cancellation = new CancellationToken())
        {
            var tasks = _validations.Select(v => _validatorFactory.GetValidator(v.ValidatorType).ValidateAsync(v.ValidationContext, cancellation));
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
            var results = _validations.Select(v => _validatorFactory.GetValidator(v.ValidatorType).Validate(v.ValidationContext)).ToArray();

            return results.Errors();
        }
    }
}
