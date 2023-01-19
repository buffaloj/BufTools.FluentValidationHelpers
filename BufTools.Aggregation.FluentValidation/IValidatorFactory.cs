using FluentValidation;
using System;

namespace MultiValidation
{
    /// <summary>
    /// An interface to create <see cref="IValidator"/> instances
    /// </summary>
    public interface IValidatorFactory
    {
        /// <summary>
        /// Gets an instance of a validator
        /// </summary>
        /// <param name="type">The type of validator to get</param>
        /// <returns>A <see cref="IValidator"/> instance</returns>
        IValidator GetValidator(Type type);
    }
}