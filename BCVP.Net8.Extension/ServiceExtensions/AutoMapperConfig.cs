using AutoMapper;

namespace BCVP.Net8.Extension.ServiceExtensions
{
    /// <summary>
    /// 靜態 全域設定 AutoMapper 設定檔案
    /// </summary>
    public class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CustomProfile());
            });
        }
    }
}
