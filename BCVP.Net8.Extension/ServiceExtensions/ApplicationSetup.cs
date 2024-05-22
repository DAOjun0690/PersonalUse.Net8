using BCVP.Net8.Common.Core;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace BCVP.Net8.Extension.ServiceExtensions
{
    public static class ApplicationSetup
    {
        public static void UseApplicationSetup(this WebApplication app)
        {
            // 當應用程式啟動前，初始化的動作
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                App.IsRun = true;
            });

            // 應用程式停止時 做的動作
            app.Lifetime.ApplicationStopped.Register(() =>
            {
                App.IsRun = false;

                //清除日志
                Log.CloseAndFlush();
            });
        }
    }

}
