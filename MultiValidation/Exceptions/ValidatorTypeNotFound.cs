using System;

namespace MultiValidation.Exceptions
{
    /// <summary>
    /// Thrown if the type of a validator cannot be found in the service collection
    /// </summary>
    public class ValidatorTypeNotFound : Exception
    {
        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="message">The reason behind the exception</param>
        public ValidatorTypeNotFound(string message) : base(message)
        {
        }
    }
}
