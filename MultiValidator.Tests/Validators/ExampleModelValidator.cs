using FluentValidation;
using MultiValidation.Tests.Models;

namespace MultiValidation.Tests.Validators
{
    public class ExampleModelValidator : AbstractValidator<ExampleModel>
    {
        public ExampleModelValidator()
        {
            RuleFor(x => x.ExampleString).NotNull().WithMessage($"{nameof(ExampleModel.ExampleString)} cannot be null");
            RuleFor(x => x.ExampleInt).NotNull().WithMessage($"{nameof(ExampleModel.ExampleString)} cannot be null");
            RuleFor(x => x.ExampleFloat).NotNull().WithMessage($"{nameof(ExampleModel.ExampleString)} cannot be null");
            RuleFor(x => x.ExampleBool).NotNull().WithMessage($"{nameof(ExampleModel.ExampleString)} cannot be null");
        }
    }
}
