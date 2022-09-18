using System;

namespace FluentValidation.Extensions.Exceptions
{
    /// <summary>
    /// Thrown if the type of a validator is not actually a validator
    /// </summary>
    public class TypeIsNotAValidatorException : Exception
    {
        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="message">The reason behind the exception</param>
        public TypeIsNotAValidatorException(string message) : base(message)
        {
        }
    }
}
