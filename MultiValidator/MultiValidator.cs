using Microsoft.Extensions.DependencyInjection;
using System;

namespace FluentValidation.Extensions
{
    public class MultiValidator
    {
        private readonly IValidatorFactory _validatorFactory;

        public MultiValidator(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
        }

        public ContextBuilder<TObj> Check<TObj>(TObj toValidate)
            where TObj : class
        {   
            var context = new Context(new ValidationContext<TObj>(toValidate), false);
            var builder = new ValidationBuilder(context, _validatorFactory);
            return new ContextBuilder<TObj>(context, builder);
        }

        public ContextBuilder<TObj> CheckOptional<TObj>(TObj? toValidate)
           where TObj : class
        {
            var context = new Context((toValidate != null) ? new ValidationContext<TObj>(toValidate) : null, true);
            var builder = new ValidationBuilder(context, _validatorFactory);
            return new ContextBuilder<TObj>(context, builder);
        }
    }
}
