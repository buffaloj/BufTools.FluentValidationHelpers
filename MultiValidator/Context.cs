using System;

namespace FluentValidation.Extensions
{
    /// <summary>
    /// A class used to represent a validation that will be performed
    /// </summary>
    internal class Context
    {
        /// <summary>
        /// Constructs an instance of an object
        /// </summary>
        /// <param name="validationContext">The fluent validation context to use</param>
        /// <param name="isOptional">true if this is for an optional paramater.  in other words, the object to validate can be null</param>
        internal Context(IValidationContext? validationContext, bool isOptional)
        {
            ValidationContext = validationContext;
            IsOptional = isOptional;
        }

        /// <summary>
        /// The type of the validator to validate the object with
        /// </summary>
        public Type? ValidatorType { get; set; }

        /// <summary>
        /// The validation context to feed to the validator
        /// </summary>
        public IValidationContext? ValidationContext { get; set; }

        /// <summary>
        /// Indicator on if this is optional. In other words, should validation be skipped for null objects
        /// </summary>
        public bool IsOptional { get; set; }
    }
}
