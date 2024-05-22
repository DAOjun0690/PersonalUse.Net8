using AutoMapper;
using BCVP.Net8.Model;

namespace BCVP.Net8
{
    public class CustomProfile : Profile
    {
        /// <summary>
        /// 建構子，用來建立關係映射
        /// </summary>
        public CustomProfile()
        {
            CreateMap<Role, RoleVo>()
                .ForMember(a => a.RoleName, b => b.MapFrom(c => c.Name));
            CreateMap<RoleVo, Role>()
                .ForMember(a => a.Name, b => b.MapFrom(c => c.RoleName));
        }
    }
}
