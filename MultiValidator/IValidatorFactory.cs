using System;

namespace FluentValidation.Extensions
{
    public interface IValidatorFactory
    {
        IValidator GetValidator(Type? type);
    }
}
