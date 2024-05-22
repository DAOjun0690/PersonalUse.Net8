using AutoMapper;
using BCVP.Net8.IService;
using BCVP.Net8.Model;
using BCVP.Net8.Service;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// ÄÝ©Ê¨Ì¿àª`¤J
        /// </summary>
        public IBaseService<Role, RoleVo> _roleServiceObj { get; set; }

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger, 
            IBaseService<Role, RoleVo> roleService, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _roleService = roleService;
            _scopeFactory = scopeFactory;
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

            var roleList = await _roleServiceObj.Query();

            return roleList;
        }
    }
}
