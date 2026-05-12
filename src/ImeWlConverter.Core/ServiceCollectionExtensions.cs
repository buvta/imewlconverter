using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Core.CodeGeneration;
using ImeWlConverter.Core.CodeGeneration.Generators;
using ImeWlConverter.Core.Language;
using ImeWlConverter.Core.Pipeline;
using ImeWlConverter.Core.WordRank;
using Microsoft.Extensions.DependencyInjection;

namespace ImeWlConverter.Core;

/// <summary>
/// Extension methods for registering ImeWlConverter.Core services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all core conversion services: pipeline, code generators, filters, converters.
    /// </summary>
    public static IServiceCollection AddImeWlConverterCore(this IServiceCollection services)
    {
        // Pipeline
        services.AddSingleton<ConversionPipeline>();
        services.AddSingleton<IConversionPipeline>(sp => sp.GetRequiredService<ConversionPipeline>());
        services.AddSingleton<CodeGenerationService>();

        // Chinese converter
        services.AddSingleton<IChineseConverter, ChineseConverter>();

        // Word rank generator
        services.AddSingleton<IWordRankGenerator, DefaultWordRankGenerator>();

        // Code generators
        services.AddSingleton<ICodeGenerator, PinyinCodeGenerator>();
        services.AddSingleton<ICodeGenerator, TerraPinyinCodeGenerator>();
        services.AddSingleton<ICodeGenerator, Wubi86CodeGenerator>();
        services.AddSingleton<ICodeGenerator, Wubi98CodeGenerator>();
        services.AddSingleton<ICodeGenerator, WubiNewAgeCodeGenerator>();
        services.AddSingleton<ICodeGenerator, ZhengmaCodeGenerator>();
        services.AddSingleton<ICodeGenerator, Cangjie5CodeGenerator>();
        services.AddSingleton<ICodeGenerator, ZhuyinCodeGenerator>();
        services.AddSingleton<ICodeGenerator, ChaoyinCodeGenerator>();
        services.AddSingleton<ICodeGenerator, QingsongErbiCodeGenerator>();
        services.AddSingleton<ICodeGenerator, ChaoqiangErbiCodeGenerator>();
        services.AddSingleton<ICodeGenerator, XiandaiErbiCodeGenerator>();
        services.AddSingleton<ICodeGenerator, YinxingErbiCodeGenerator>();

        return services;
    }
}
