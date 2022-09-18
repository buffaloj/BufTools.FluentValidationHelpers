using System;

namespace FluentValidation.Extensions
{
    internal class Context
    {
        internal Context(IValidationContext? validationContext, bool isOptional)
        {
            ValidationContext = validationContext;
            IsOptional = isOptional;
        }

        public Type? ValidatorType { get; set; }
        public IValidationContext? ValidationContext { get; set; }
        public bool IsOptional { get; set; }
    }
}
