using System;

namespace FluentValidation.Extensions
{
    internal class Context
    {
        internal Context(IValidator validator, IValidationContext? context)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            ValidationContext = context;
        }

        public IValidator Validator { get; set; }
        public IValidationContext? ValidationContext { get; set; }
    }
}
