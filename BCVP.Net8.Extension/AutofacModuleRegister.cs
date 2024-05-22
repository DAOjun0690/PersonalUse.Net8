using Autofac;
using BCVP.Net8.IService;
using BCVP.Net8.Repository;
using BCVP.Net8.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BCVP.Net8.Extension
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;

            var servicesDllFile = Path.Combine(basePath, "BCVP.Net8.Service.dll");
            var repositoryDllFile = Path.Combine(basePath, "BCVP.Net8.Repository.dll");

            builder.RegisterGeneric(typeof(BaseRepositroy<>)).As(typeof(IBaseRepositroy<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(BaseService<,>)).As(typeof(IBaseService<,>)).InstancePerLifetimeScope();

            // 獲得 Service.dll 程式服務，並註冊
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);
            builder.RegisterAssemblyTypes(assemblysServices)
                .AsImplementedInterfaces()
                .InstancePerDependency() // 瞬時注入
                .PropertiesAutowired();

            // 獲得 Repository.dll 程式服務，並註冊
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();
        }
    }
}
