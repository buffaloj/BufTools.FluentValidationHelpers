using FluentValidation.Extensions.Exceptions;
using FluentValidation.Extensions.Resources;
using System;

namespace FluentValidation.Extensions
{
    /// <inheritdoc/>
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public IValidator GetValidator(Type? type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var validator = _serviceProvider.GetService(type);
            if (validator == null)
            {
                throw new ValidatorTypeNotFoundException(string.Format(MultiValidatorResources.ValidatorTypeNotFoundFormat, type.Name));
            }

            if (!(validator is IValidator))
            {
                throw new TypeIsNotAValidatorException(string.Format(MultiValidatorResources.TypeIsNotAValidatorFormat, type.Name));
            }

            return (IValidator)validator;
        }
    }
}
