using System;

namespace MultiValidation.Exceptions
{
    /// <summary>
    /// Thrown if a non-optional object to be validated is null
    /// </summary>
    public class NonOptionalNullValidation : Exception
    {
        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="message">The reason behind the exception</param>
        public NonOptionalNullValidation(string message) : base(message)
        {
        }
    }
}
