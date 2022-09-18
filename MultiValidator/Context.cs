namespace FluentValidation.Extensions
{
    internal class Context
    {
        internal Context(IValidationContext? validationContext, bool isOptional)
        {
            ValidationContext = validationContext;
            IsOptional = isOptional;
        }

        public IValidator? Validator { get; set; }
        public IValidationContext? ValidationContext { get; set; }
        public bool IsOptional { get; set; }
    }
}
