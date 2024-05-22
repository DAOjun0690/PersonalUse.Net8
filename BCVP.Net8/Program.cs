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
            var builder = WebApplication.CreateBuilder(args); // �إߤ@��WEB APP
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder => {
                    builder.RegisterModule<AutofacModuleRegister>();
                    builder.RegisterModule<AutofacPropertityModuleReg>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    hostingContext.Configuration.ConfigureApplication(); // ���� AppSetting
                })
                ;
            builder.ConfigureApplication(); // ���� Web�BHost�B����
            // Add services to the container.
            //builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            builder.Services.AddControllers()
                .AddControllersAsServices();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); // OPEN API�ɮ�

            // �[�J AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMappings();

            // �̿�`�J
            //builder.Services.AddScoped(typeof(IBaseRepositroy<>),typeof(BaseRepositroy<>));
            //builder.Services.AddScoped(typeof(IBaseService<,>),typeof(BaseService<,>));

            // appSetting�]�w
            builder.Services.AddSingleton(new AppSettings(builder.Configuration));
            //// �P�W���]�w�� ConfigureAppConfiguration �N��ۦP
            //ConfigurableOptions.ConfigureApplication(builder.Configuration); 
            builder.Services.AddAllOptionRegister();


             var app = builder.Build();
            app.ConfigureApplication(); // ���� Service
            app.UseApplicationSetup(); // ���U �������ε{���O�_�Ұ� ���P�_�I

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
