using System;

namespace FluentValidation.Extensions.Exceptions
{
    /// <summary>
    /// Thrown if a non-optional object to be validated is null
    /// </summary>
    public class NullObjectException : Exception
    {
        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="message">The reason behind the exception</param>
        public NullObjectException(string message) : base(message)
        {
        }
    }
}
