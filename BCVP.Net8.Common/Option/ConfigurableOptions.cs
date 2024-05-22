using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BCVP.Net8.Common.Option.Core;

public static class ConfigurableOptions
{
    internal static IConfiguration Configuration;
    public static void ConfigureApplication(this IConfiguration configuration)
    {
        Configuration = configuration;
    }


    /// <summary>增加選項設定</summary>
    /// <typeparam name="TOptions">選項類型</typeparam>
    /// <param name="services">服務集合</param>
    /// <returns>服務集合</returns>
    public static IServiceCollection AddConfigurableOptions<TOptions>(this IServiceCollection services)
        where TOptions : class, IConfigurableOptions
    {
        Type optionsType = typeof(TOptions);
        string path = GetConfigurationPath(optionsType);
        services.Configure<TOptions>(Configuration.GetSection(path));

        return services;
    }

    public static IServiceCollection AddConfigurableOptions(this IServiceCollection services, Type type)
    {
        string path = GetConfigurationPath(type);
        var config = Configuration.GetSection(path);

        Type iOptionsChangeTokenSource = typeof(IOptionsChangeTokenSource<>);
        Type iConfigureOptions = typeof(IConfigureOptions<>);
        Type configurationChangeTokenSource = typeof(ConfigurationChangeTokenSource<>);
        Type namedConfigureFromConfigurationOptions = typeof(NamedConfigureFromConfigurationOptions<>);
        iOptionsChangeTokenSource = iOptionsChangeTokenSource.MakeGenericType(type);
        iConfigureOptions = iConfigureOptions.MakeGenericType(type);
        configurationChangeTokenSource = configurationChangeTokenSource.MakeGenericType(type);
        namedConfigureFromConfigurationOptions = namedConfigureFromConfigurationOptions.MakeGenericType(type);

        services.AddOptions();
        services.AddSingleton(iOptionsChangeTokenSource,
            Activator.CreateInstance(configurationChangeTokenSource, Options.DefaultName, config) ?? throw new InvalidOperationException());
        return services.AddSingleton(iConfigureOptions,
            Activator.CreateInstance(namedConfigureFromConfigurationOptions, Options.DefaultName, config) ?? throw new InvalidOperationException());
    }

    /// <summary>獲得配置路徑</summary>
    /// <param name="optionsType">選項類型</param>
    /// <returns></returns>
    public static string GetConfigurationPath(Type optionsType)
    {
        var endPath = new[] { "Option", "Options" };
        var configurationPath = optionsType.Name;
        foreach (var s in endPath)
        {
            if (configurationPath.EndsWith(s))
            {
                return configurationPath[..^s.Length];
            }
        }

        return configurationPath;
    }
}
