using FluentValidation;

namespace MultiValidation.Tests.Validators
{
    public class ExampleNullableIntValidator : AbstractValidator<int?>
    {
        public ExampleNullableIntValidator()
        {
            RuleFor(i => i).NotNull().GreaterThan(0).WithMessage("Must be greater than zero");
        }
    }
}
