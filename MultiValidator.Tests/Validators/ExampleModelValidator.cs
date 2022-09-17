using FluentValidation.Extensions.Tests.Models;

namespace FluentValidation.Extensions.Tests.Validators
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
