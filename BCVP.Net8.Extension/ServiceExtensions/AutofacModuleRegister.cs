using Autofac;
using Autofac.Extras.DynamicProxy;
using BCVP.Net8.IService;
using BCVP.Net8.Repository;
using BCVP.Net8.Service;
using System.Reflection;

namespace BCVP.Net8.Extension.ServiceExtensions
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;

            var servicesDllFile = Path.Combine(basePath, "BCVP.Net8.Service.dll");
            var repositoryDllFile = Path.Combine(basePath, "BCVP.Net8.Repository.dll");

            // 註冊 AOP
            var aopType = new List<Type>() { typeof(ServiceAOP) };
            builder.RegisterType<ServiceAOP>();

            builder.RegisterGeneric(typeof(BaseRepositroy<>)).As(typeof(IBaseRepositroy<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(BaseService<,>)).As(typeof(IBaseService<,>))
                .InstancePerLifetimeScope()
                .EnableInterfaceInterceptors()
                .InterceptedBy(aopType.ToArray());

            // 獲得 Service.dll 程式服務，並註冊
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);
            builder.RegisterAssemblyTypes(assemblysServices)
                .AsImplementedInterfaces()
                .InstancePerDependency() // 瞬時注入
                .PropertiesAutowired()
                .EnableInterfaceInterceptors() // AOP 注入使用
                .InterceptedBy(aopType.ToArray()); // AOP 注入使用

            // 獲得 Repository.dll 程式服務，並註冊
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository)
                .AsImplementedInterfaces()
                .InstancePerDependency() // 瞬時注入
                .PropertiesAutowired();
        }
    }
}
