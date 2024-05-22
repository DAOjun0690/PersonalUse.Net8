using AutoMapper;
using BCVP.Net8.Common;
using BCVP.Net8.Common.Core;
using BCVP.Net8.Common.Option;
using BCVP.Net8.IService;
using BCVP.Net8.Model;
using BCVP.Net8.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BCVP.Net8.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IBaseService<Role, RoleVo> _roleService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<RedisOptions> _redisOptions;

        /// <summary>
        /// ÄÝ©Ê¨Ì¿àª`¤J
        /// </summary>
        public IBaseService<Role, RoleVo> _roleServiceObj { get; set; }

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger, 
            IBaseService<Role, RoleVo> roleService, 
            IServiceScopeFactory scopeFactory,
            IOptions<RedisOptions> redisOptions)
        {
            _logger = logger;
            _roleService = roleService;
            _scopeFactory = scopeFactory;
            _redisOptions = redisOptions;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<object> Get()
        {
            //var userService = new UserService();
            //var userList = await userService.Query();
            //return userList;

            //var roleService = new BaseService<Role, RoleVo>(_mapper);
            //var roleList = await roleService.Query();
            //return roleList;

            //var roleList = await _roleService.Query();
            //Console.WriteLine(_roleService.GetHashCode());
            //var roleList2 = await _roleService.Query();
            //Console.WriteLine(_roleService.GetHashCode());

            //using var scope = _scopeFactory.CreateScope();
            //var _dataStaticService = 
            //    scope.ServiceProvider.GetRequiredService<IBaseService<Role, RoleVo>>();
            //var roleList = await _dataStaticService.Query();
            //var _dataStaticService2 =
            //    scope.ServiceProvider.GetRequiredService<IBaseService<Role, RoleVo>>();
            //var roleList2 = await _dataStaticService2.Query();

            //var roleList = await _roleServiceObj.Query();
            //var redisEnable = AppSettings.app(new string[] { "Redis", "Enable" });
            //var redisConnectionString = AppSettings.GetValue("Redis:ConnectionString");

            //Console.WriteLine($"Enable: {redisEnable}");
            //Console.WriteLine($"redisConnectionString: {redisConnectionString}");

            //var redisOptions = _redisOptions.Value;
            //Console.WriteLine(JsonConvert.SerializeObject(redisOptions));

            var roleServiceObjNew = App.GetService<IBaseService<Role, RoleVo>>(false);
            var roleList = await roleServiceObjNew.Query();
            var redisOptions = App.GetOptions<RedisOptions>();


            Console.WriteLine("api request end ...");
            return roleList;
        }
    }
}
