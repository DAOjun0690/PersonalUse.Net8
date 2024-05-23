using BCVP.Net8.Common.Option;
using BCVP.Net8.Common.Option.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace BCVP.Net8.Common.Core
{
    public class App
    {
        static App()
        {
            EffectiveTypes = Assemblies.SelectMany(GetTypes);
        }

        private static bool _isRun;

        /// <summary>是否正在執行</summary>
        public static bool IsBuild { get; set; }

        public static bool IsRun
        {
            get => _isRun;
            set => _isRun = IsBuild = value;
        }

        /// <summary>應用有效程式</summary>
        public static readonly IEnumerable<Assembly> Assemblies = RuntimeExtension.GetAllAssemblies();

        /// <summary>有效程式類型</summary>
        public static readonly IEnumerable<Type> EffectiveTypes;

        /// <summary>優先使用App.GetService()手動獲取服務</summary>
        public static IServiceProvider RootServices => IsRun || IsBuild ? InternalApp.RootServices : null;

        /// <summary>獲得Web環境，ex: 是否是開發環境、Release環境等</summary>
        public static IWebHostEnvironment WebHostEnvironment => InternalApp.WebHostEnvironment;

        /// <summary>獲得泛型環境，ex: 是否是開發環境、Release環境等</summary>
        public static IHostEnvironment HostEnvironment => InternalApp.HostEnvironment;

        /// <summary>全域配置選項</summary>
        public static IConfiguration Configuration => InternalApp.Configuration;

        /// <summary>
        /// 獲得request上下文
        /// </summary>
        public static HttpContext HttpContext => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext;

        //public static IUser User => GetService<IUser>();

        #region Service

        /// <summary>解析服務提供器</summary>
        /// <param name="serviceType"></param>
        /// <param name="mustBuild"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static IServiceProvider GetServiceProvider(Type serviceType, bool mustBuild = false, bool throwException = true)
        {
            if (HostEnvironment == null || RootServices != null &&
                InternalApp.InternalServices
                    .Where(u =>
                        u.ServiceType ==
                        (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType))
                    .Any(u => u.Lifetime == ServiceLifetime.Singleton))
                return RootServices;

            // 獲得請求生命週期的服務
            if (HttpContext?.RequestServices != null)
                return HttpContext.RequestServices;

            if (RootServices != null)
            {
                IServiceScope scope = RootServices.CreateScope();
                return scope.ServiceProvider;
            }

            if (mustBuild)
            {
                if (throwException)
                {
                    throw new ApplicationException("當前不可用，必須等到WebApplication Build之後。");
                }

                return default;
            }

            ServiceProvider serviceProvider = InternalApp.InternalServices.BuildServiceProvider();
            return serviceProvider;
        }

        public static TService GetService<TService>(bool mustBuild = true) where TService : class =>
            GetService(typeof(TService), null, mustBuild) as TService;

        /// <summary>獲得request生命週期的服務</summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="mustBuild"></param>
        /// <returns></returns>
        public static TService GetService<TService>(IServiceProvider serviceProvider, bool mustBuild = true)
            where TService : class => (serviceProvider ?? GetServiceProvider(typeof(TService), mustBuild, false))?.GetService<TService>();

        /// <summary>獲得request生命週期的服務</summary>
        /// <param name="type"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="mustBuild"></param>
        /// <returns></returns>
        public static object GetService(Type type, IServiceProvider serviceProvider = null, bool mustBuild = true) =>
            (serviceProvider ?? GetServiceProvider(type, mustBuild, false))?.GetService(type);

        #endregion

        #region private

        /// <summary>載入程式中的所有類型</summary>
        /// <param name="ass"></param>
        /// <returns></returns>
        private static IEnumerable<Type> GetTypes(Assembly ass)
        {
            Type[] source = Array.Empty<Type>();
            try
            {
                source = ass.GetTypes();
            }
            catch
            {
                Console.WriteLine($@"Error load `{ass.FullName}` assembly.");
            }

            return source.Where(u => u.IsPublic);
        }

        #endregion

        #region Options

        /// <summary>獲得配置</summary>
        /// <typeparam name="TOptions">強型別選項類別</typeparam>
        /// <returns>TOptions</returns>
        public static TOptions GetConfig<TOptions>()
            where TOptions : class, IConfigurableOptions
        {
            TOptions instance = Configuration
                .GetSection(ConfigurableOptions.GetConfigurationPath(typeof(TOptions)))
                .Get<TOptions>();
            return instance;
        }

        /// <summary>獲得選項</summary>
        /// <typeparam name="TOptions">強型別選項類別</typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns>TOptions</returns>
        public static TOptions GetOptions<TOptions>(IServiceProvider serviceProvider = null) where TOptions : class, new()
        {
            IOptions<TOptions> service = GetService<IOptions<TOptions>>(serviceProvider ?? RootServices, false);
            return service?.Value;
        }

        /// <summary>獲得選項</summary>
        /// <typeparam name="TOptions">強型別選項類別</typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns>TOptions</returns>
        public static TOptions GetOptionsMonitor<TOptions>(IServiceProvider serviceProvider = null)
            where TOptions : class, new()
        {
            IOptionsMonitor<TOptions> service =
                GetService<IOptionsMonitor<TOptions>>(serviceProvider ?? RootServices, false);
            return service?.CurrentValue;
        }

        /// <summary>獲得選項</summary>
        /// <typeparam name="TOptions">強型別選項類別</typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns>TOptions</returns>
        public static TOptions GetOptionsSnapshot<TOptions>(IServiceProvider serviceProvider = null)
            where TOptions : class, new()
        {
            IOptionsSnapshot<TOptions> service = GetService<IOptionsSnapshot<TOptions>>(serviceProvider, false);
            return service?.Value;
        }

        #endregion
    }

}
