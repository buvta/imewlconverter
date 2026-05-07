using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Core.CodeGeneration;
using ImeWlConverter.Core.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace ImeWlConverter.Core;

/// <summary>
/// Extension methods for registering ImeWlConverter.Core services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers core conversion pipeline services including
    /// <see cref="IConversionPipeline"/>, <see cref="CodeGenerationService"/>, and <see cref="FilterPipeline"/>.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddImeWlConverterCore(this IServiceCollection services)
    {
        services.AddSingleton<IConversionPipeline, ConversionPipeline>();
        services.AddSingleton<CodeGenerationService>();
        services.AddSingleton<FilterPipeline>();
        return services;
    }
}
