using BCVP.Net8.Model;
using Newtonsoft.Json;

namespace BCVP.Net8.Repository
{
    public class BaseRepositroy<TEntity> : IBaseRepositroy<TEntity> where TEntity : class
    {
        public async Task<List<TEntity>> Query()
        {
            await Task.CompletedTask;
            var data = "[{\"Id\":18,\"Name\":\"a,smnd,amsnd\"}]";

            return JsonConvert.DeserializeObject<List<TEntity>>(data) ?? new List<TEntity>();
        }
    }
}
