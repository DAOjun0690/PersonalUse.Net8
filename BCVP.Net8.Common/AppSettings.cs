using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCVP.Net8.Common
{
    public class AppSettings
    {
        public static IConfiguration Configuration { get; set; }
        static string contentPath { get; set; }

        public AppSettings(string contentPath)
        {
            string Path = "appsettings.json";

            // 如果是把appSetting 是根據環境變數分開來的話 可以用以下寫法
            //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

            Configuration = new ConfigurationBuilder()
                .SetBasePath(contentPath)
                .Add(new JsonConfigurationSource
                {
                    Path = Path,
                    Optional = false,
                    ReloadOnChange = true
                }) // 這樣的話，可以直接讀取資料夾裡的json檔案，而不是 bin 資料夾底下的，所以不用修改複製屬性
                .Build();
        }

        public AppSettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 封裝要操作的屬性
        /// </summary>
        /// <param name="sections">節點設定</param>
        /// <returns></returns>
        public static string app(params string[] sections)
        {
            try
            {
                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception)
            {
            }

            return "";
        }

        /// <summary>
        /// 遞迴獲得appSetting設定資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<T> app<T>(params string[] sections)
        {
            List<T> list = new List<T>();
            // 引用 Microsoft.Extensions.Configuration.Binder package
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }


        /// <summary>
        /// 根據路徑  configuration["App:Name"];
        /// </summary>
        /// <param name="sectionsPath"></param>
        /// <returns></returns>
        public static string GetValue(string sectionsPath)
        {
            try
            {
                return Configuration[sectionsPath];
            }
            catch (Exception)
            {
            }

            return "";
        }
    }
}
