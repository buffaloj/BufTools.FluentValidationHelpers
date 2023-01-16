using FluentValidation;

namespace MultiValidation.Tests.Validators
{
    public class ExampleIntValidator : AbstractValidator<int>
    {
        public ExampleIntValidator()
        {
            RuleFor(i => i).GreaterThan(0).WithMessage("Must be greater than zero");
        }
    }
}
