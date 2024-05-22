using BCVP.Net8.IService;
using BCVP.Net8.Repository;
using BCVP.Net8.Service;

namespace BCVP.Net8
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args); // 建立一個WEB APP

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); // OPEN API檔案

            // 加入 AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMappings();

            builder.Services.AddScoped(typeof(IBaseRepositroy<>),typeof(BaseRepositroy<>));
            builder.Services.AddScoped(typeof(IBaseService<,>),typeof(BaseService<,>));

             var app = builder.Build();

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
