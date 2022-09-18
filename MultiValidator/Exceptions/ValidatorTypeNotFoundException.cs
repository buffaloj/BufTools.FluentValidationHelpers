using System;

namespace FluentValidation.Extensions.Exceptions
{
    public class ValidatorTypeNotFoundException : Exception
    {
        public ValidatorTypeNotFoundException(string message) : base(message)
        {
        }
    }
}
