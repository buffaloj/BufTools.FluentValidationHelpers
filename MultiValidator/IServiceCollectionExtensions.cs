using Microsoft.Extensions.DependencyInjection;

namespace MultiValidation
{
    /// <summary>
    /// A helper class to register a <see cref="MultiValidator"/> for use with dependency injection
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a <see cref="MultiValidator"/> with a service collection so it can be dependency injected
        /// </summary>
        /// <param name="services"></param>
        public static void AddMultiValidation(this IServiceCollection services)
        {
            services.AddSingleton<MultiValidator>();
            services.AddSingleton<IValidatorFactory, ValidatorFactory>();
        }
    }
}
