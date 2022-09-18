using System;

namespace FluentValidation.Extensions
{
    /// <summary>
    /// A builder used to fill in context values for a validation
    /// </summary>
    /// <typeparam name="TObj"></typeparam>
    public class ContextBuilder<TObj>
        where TObj : class
    {
        private readonly ValidationBuilder _builder;
        private readonly Context _context;

        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="context"></param>
        /// <param name="builder"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal ContextBuilder(Context context, ValidationBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Supplies the type of validator to use
        /// </summary>
        /// <typeparam name="TValidator">The type of the validator to fetch for use</typeparam>
        /// <returns>A <see cref="ValidationBuilder"/> instance to continue validation construction</returns>
        public ValidationBuilder Using<TValidator>()
            where TValidator : IValidator<TObj>
        {
            _context.ValidatorType = typeof(TValidator);
            return _builder;
        }
    }
}
