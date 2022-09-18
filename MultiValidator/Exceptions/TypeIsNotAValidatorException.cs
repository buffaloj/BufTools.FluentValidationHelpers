using System;

namespace FluentValidation.Extensions.Exceptions
{
    public class TypeIsNotAValidatorException : Exception
    {
        public TypeIsNotAValidatorException(string message) : base(message)
        {
        }
    }
}
