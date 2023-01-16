using System;

namespace MultiValidation.Exceptions
{
    /// <summary>
    /// Thrown if the type of a validator is not actually a validator
    /// </summary>
    public class TypeIsNotAValidator : Exception
    {
        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="message">The reason behind the exception</param>
        public TypeIsNotAValidator(string message) : base(message)
        {
        }
    }
}
