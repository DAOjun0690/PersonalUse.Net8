using Autofac;
using Microsoft.AspNetCore.Mvc;

namespace BCVP.Net8.Extensions
{
    public class AutofacPropertityModuleReg : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired(); // 關鍵此row 要知道屬性註冊的名稱

        }
    }
}
