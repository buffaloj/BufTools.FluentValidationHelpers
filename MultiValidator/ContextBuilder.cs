using System;

namespace FluentValidation.Extensions
{
    public class ContextBuilder<TObj>
        where TObj : class
    {
        private readonly ValidationBuilder _builder;
        private readonly Context _context;

        internal ContextBuilder(Context context, ValidationBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ValidationBuilder Using<TValidator>()
            where TValidator : IValidator<TObj>, new()
        {
            _context.Validator = new TValidator();
            return _builder;
        }
    }
}
