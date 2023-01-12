using FluentValidation;
using MultiValidation.Exceptions;

namespace MultiValidation
{
    /// <summary>
    /// A class used to build up a group of validations to perform as a set
    /// </summary>
    public class MultiValidator
    {
        private readonly IValidatorFactory _factory;

        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="validatorFactory">A factory to use to create <see cref="IValidator"/> instances</param>
        public MultiValidator(IValidatorFactory validatorFactory) => _factory = validatorFactory;

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
            var aggregator = new ValidationAggregator(_factory);
            return aggregator.For(instance);
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
            var aggregator = new ValidationAggregator(_factory);
            return aggregator.ForOptional(instance);
        }
    }
}
