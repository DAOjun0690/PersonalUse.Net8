using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BCVP.Net8.Common.Core
{
    public static class InternalApp
    {
        internal static IServiceCollection InternalServices;

        /// <summary>root服務</summary>
        internal static IServiceProvider RootServices;

        /// <summary>獲得Web環境</summary>
        internal static IWebHostEnvironment WebHostEnvironment;

        /// <summary>獲得泛型環境</summary>
        internal static IHostEnvironment HostEnvironment;

        /// <summary>配置物件</summary>
        internal static IConfiguration Configuration;

        public static void ConfigureApplication(this WebApplicationBuilder web)
        {
            HostEnvironment = web.Environment;
            WebHostEnvironment = web.Environment;
            InternalServices = web.Services;
        }

        public static void ConfigureApplication(this IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static void ConfigureApplication(this IHost app)
        {
            RootServices = app.Services;
        }
    }

}
