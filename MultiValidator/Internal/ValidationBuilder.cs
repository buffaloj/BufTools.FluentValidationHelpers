﻿using System;

namespace FluentValidation.Extensions
{
    /// <summary>
    /// A class that builds a validation and adds it to a <see cref="ValidationAggregator"/>
    /// </summary>
    /// <typeparam name="TObj">The type of the object to be validated</typeparam>
    public class ValidationBuilder<TObj>
        where TObj : class
    {
        private readonly ValidationAggregator _aggregator;
        private readonly bool _isOptional;
        private readonly TObj _instance;

        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="aggregator">The aggregator to add validations to</param>
        /// <param name="instance">The object instance to validate</param>
        /// <param name="isOptional">Indicates of null instance values are ok and can be skipped</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal ValidationBuilder(ValidationAggregator aggregator, TObj instance, bool isOptional)
        {
            _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            _instance = instance;
            if (!isOptional && _instance == null)
                throw new ArgumentNullException(nameof(instance));
            _isOptional = isOptional;
        }

        /// <summary>
        /// Supplies the type of <see cref="IValidator"/> to use in the validation
        /// </summary>
        /// <typeparam name="TValidator">The type of <see cref="IValidator" to perform the validation/></typeparam>
        /// <returns>A class to use in adding more validations to the chain</returns>
        public ValidationAggregator Use<TValidator>()
            where TValidator : IValidator<TObj> 
        {
            if (_isOptional && _instance == null)
                return _aggregator;

            var context = new ValidationContext<TObj>(_instance);
            var validation = new Validation(context, typeof(TValidator));
            return _aggregator.AddValidation(validation);
        }
    }
}
