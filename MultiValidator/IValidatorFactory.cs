using System;

namespace FluentValidation.Extensions
{
    /// <summary>
    /// A factory interface to create instances of a validator
    /// </summary>
    public interface IValidatorFactory
    {
        /// <summary>
        /// Fetches an instance of a validator
        /// </summary>
        /// <param name="type">The type the validator must be</param>
        /// <returns>A validator instance</returns>
        IValidator GetValidator(Type? type);
    }
}
