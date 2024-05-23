using BCVP.Net8.Common.Option;
using BCVP.Net8.Common.Option.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BCVP.Net8.Extension.ServiceExtensions
{
    public static class AllOptionRegister
    {
        public static void AddAllOptionRegister(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            foreach (var optionType in typeof(ConfigurableOptions).Assembly.GetTypes().Where(s =>
                         !s.IsInterface && typeof(IConfigurableOptions).IsAssignableFrom(s)))
            {
                services.AddConfigurableOptions(optionType);
            }
        }
    }

}
