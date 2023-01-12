using FluentValidation;
using System;

namespace MultiValidator
{
    /// <summary>
    /// A storage class to hold the object instance to validate and the type of validator to use
    /// </summary>
    public class Validation
    {
        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="context">The validation context to evaluate</param>
        /// <param name="validatorType">The type of validator to use</param>
        internal Validation(IValidationContext context, Type validatorType)
        {
            ValidationContext = context;
            ValidatorType = validatorType;
        }

        /// <summary>
        /// The type of the validator to validate the object with
        /// </summary>
        public Type ValidatorType { get; private set; }

        /// <summary>
        /// The validation context to feed to the validator
        /// </summary>
        public IValidationContext ValidationContext { get; private set; }
    }
}
