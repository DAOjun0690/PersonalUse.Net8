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
        //private readonly IBaseService<Role, RoleVo> _roleService;
        private readonly IMapper _mapper;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
            //IBaseService<Role, RoleVo> roleService,
            IMapper mapper)
        {
            _logger = logger;
            //_roleService = roleService;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<object> Get()
        {
            //var userService = new UserService();
            //var userList = await userService.Query();
            //return userList;

            var roleService = new BaseService<Role, RoleVo>(_mapper);
            var roleList = await roleService.Query();
            return roleList;

            //var roleList2 = await _roleService.Query();
            //return roleList2;
        }
    }
}
