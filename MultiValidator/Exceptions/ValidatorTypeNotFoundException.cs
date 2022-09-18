using System;

namespace FluentValidation.Extensions.Exceptions
{
    /// <summary>
    /// Thrown if the type of a validator cannot be found in the service collection
    /// </summary>
    public class ValidatorTypeNotFoundException : Exception
    {
        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="message">The reason behind the exception</param>
        public ValidatorTypeNotFoundException(string message) : base(message)
        {
        }
    }
}
