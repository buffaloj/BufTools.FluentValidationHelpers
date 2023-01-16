using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace MultiValidation.Extensions
{
    /// <summary>
    /// Helpers extensions
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Converts an array of validation results and returns a collection of <see cref="ValidationFailure"/>
        /// </summary>
        /// <param name="results">The validation results to get the failures from</param>
        /// <returns>A collection of validation failures</returns>
        public static IEnumerable<ValidationFailure> Errors(this ValidationResult[] results)
        {
            return results?.SelectMany(result => result.Errors)
                          ?.Where(f => f != null)
                          ?? Enumerable.Empty<ValidationFailure>();
        }
    }
}
