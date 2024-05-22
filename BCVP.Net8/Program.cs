using Autofac;
using Autofac.Extensions.DependencyInjection;
using BCVP.Net8.Common;
using BCVP.Net8.Extension.ServiceExtensions;
using BCVP.Net8.Extensions;
using BCVP.Net8.Common.Option.Core;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BCVP.Net8.Common.Core;

namespace BCVP.Net8
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args); // 建立一個WEB APP
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder => {
                    builder.RegisterModule<AutofacModuleRegister>();
                    builder.RegisterModule<AutofacPropertityModuleReg>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    hostingContext.Configuration.ConfigureApplication(); // 拿到 AppSetting
                })
                ;
            builder.ConfigureApplication(); // 拿到 Web、Host、環境
            // Add services to the container.
            //builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            builder.Services.AddControllers()
                .AddControllersAsServices();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); // OPEN API檔案

            // 加入 AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMappings();

            // 依賴注入
            //builder.Services.AddScoped(typeof(IBaseRepositroy<>),typeof(BaseRepositroy<>));
            //builder.Services.AddScoped(typeof(IBaseService<,>),typeof(BaseService<,>));

            // appSetting設定
            builder.Services.AddSingleton(new AppSettings(builder.Configuration));
            //// 與上面設定的 ConfigureAppConfiguration 意思相同
            //ConfigurableOptions.ConfigureApplication(builder.Configuration); 
            builder.Services.AddAllOptionRegister();


             var app = builder.Build();
            app.ConfigureApplication(); // 拿到 Service
            app.UseApplicationSetup(); // 註冊 偵測應用程式是否啟動 的判斷點

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
