using System;
using System.Collections.Generic;

namespace FluentValidation.Extensions
{
    public class MultiValidator
    {
        public ValidationBuilder With<TValidator, TObj>(TObj toValidate)
            where TValidator : IValidator<TObj>, new()
        {
            var validator = new TValidator();
            var validations = new List<Context> { new Context(validator, new ValidationContext<TObj>(toValidate)) };

            return new ValidationBuilder(validations);
        }

        public ValidationBuilder WithOptional<TValidator, TObj>(TObj? toValidate)
            where TValidator : IValidator<TObj>, new()
            where TObj : class
        {
            if (toValidate == null)
                return new ValidationBuilder(new List<Context>());
            else
                return With<TValidator, TObj>(toValidate);
        }
    }
}
