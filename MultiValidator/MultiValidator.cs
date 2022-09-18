using System;

namespace FluentValidation.Extensions
{
    /// <summary>
    /// A class used to build up sets of validations to perform as a group
    /// </summary>
    public class MultiValidator
    {
        private readonly IValidatorFactory _validatorFactory;

        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="validatorFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MultiValidator(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
        }

        /// <summary>
        /// Adds an object to the validation
        /// </summary>
        /// <typeparam name="TObj">The type of the object to validate</typeparam>
        /// <param name="toValidate">The object to validate</param>
        /// <returns>A <see cref="ContextBuilder{TObj}"/> instance so more info can be provided for the validation</returns>
        public ContextBuilder<TObj> Check<TObj>(TObj toValidate)
            where TObj : class
        {   
            var context = new Context(new ValidationContext<TObj>(toValidate), false);
            var builder = new ValidationBuilder(context, _validatorFactory);
            return new ContextBuilder<TObj>(context, builder);
        }

        /// <summary>
        /// Adds an optional object to the validation - the validation will pass when the object is null
        /// </summary>
        /// <typeparam name="TObj">The type of the object to validate</typeparam>
        /// <param name="toValidate">The object to validate</param>
        /// <returns>A <see cref="ContextBuilder{TObj}"/> instance so more info can be provided for the validation</returns>
        public ContextBuilder<TObj> CheckOptional<TObj>(TObj? toValidate)
           where TObj : class
        {
            var context = new Context((toValidate != null) ? new ValidationContext<TObj>(toValidate) : null, true);
            var builder = new ValidationBuilder(context, _validatorFactory);
            return new ContextBuilder<TObj>(context, builder);
        }
    }
}
