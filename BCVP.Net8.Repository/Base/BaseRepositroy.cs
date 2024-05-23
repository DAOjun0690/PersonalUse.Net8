using BCVP.Net8.Model;
using Newtonsoft.Json;

namespace BCVP.Net8.Repository
{
    public class BaseRepositroy<TEntity> : IBaseRepositroy<TEntity> where TEntity : class
    {
        public async Task<List<TEntity>> Query()
        {
            await Task.CompletedTask;
            var data = "[{\"Id\":18,\"Name\":\"恭喜你成功把我 BCVP.Net8 給打開了 賀!\"}]";

            return JsonConvert.DeserializeObject<List<TEntity>>(data) ?? new List<TEntity>();
        }
    }
}
