using FluentValidation.Extensions.Exceptions;
using FluentValidation.Extensions.Resources;
using System;

namespace FluentValidation.Extensions
{
    /// <inheritdoc/>
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IServiceProvider? _serviceProvider;

        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="serviceProvider">The provider to use to create validators</param>
        public ValidatorFactory(IServiceProvider? serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public IValidator GetValidator(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!typeof(IValidator).IsAssignableFrom(type))
                throw new TypeIsNotAValidatorException(string.Format(MultiValidatorResources.TypeIsNotAValidatorFormat, type.Name));

            var validator = (_serviceProvider != null) ? _serviceProvider.GetService(type) : Activator.CreateInstance(type);

            if (validator == null)
                throw new ValidatorTypeNotFoundException(string.Format(MultiValidatorResources.ValidatorTypeNotFoundFormat, type.Name));

            return (IValidator)validator;
        }
    }
}
