using BCVP.Net8.Common.Caches;
using BCVP.Net8.Common.Core;
using BCVP.Net8.Common.Option;
using BCVP.Net8.Extension.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BCVP.Net8.Extension.ServiceExtensions;

public static class CacheSetup
{
    /// <summary>
    /// 統一註冊 Cache
    /// </summary>
    /// <param name="services"></param>
    public static void AddCacheSetUp(this IServiceCollection services)
    {
        var cacheOptions = App.GetOptions<RedisOptions>();
        if (cacheOptions.Enable)
        {
            // 設定啟動Redis服務，雖然可能影響專案啟動速度，但是不能在執行時報錯，所以是合理的
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                // 獲得連線字串
                var configuration =
                    ConfigurationOptions.Parse(cacheOptions.ConnectionString, true);
                configuration.ResolveDns = true;
                return ConnectionMultiplexer.Connect(configuration);
            });
            services.AddSingleton<ConnectionMultiplexer>(p =>
                p.GetService<IConnectionMultiplexer>() as ConnectionMultiplexer);
            // 使用Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory =
                    () => Task.FromResult(App.GetService<IConnectionMultiplexer>(false));
                if (!string.IsNullOrEmpty(cacheOptions.InstanceName)) options.InstanceName = cacheOptions.InstanceName;
            });

            services.AddTransient<IRedisBasketRepository, RedisBasketRepository>();
        }
        else
        {
            // 使用記憶體
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
        }

        services.AddSingleton<ICaching, Caching>();
    }

}
