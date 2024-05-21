using BCVP.Net8.IService;
using BCVP.Net8.Model;
using BCVP.Net8.Repository;

namespace BCVP.Net8.Service
{
    public class BaseService<TEntity, TVo> : 
        IBaseService<TEntity, TVo> where TEntity : class, new()
    {
        

        public async Task<List<TEntity>> Query()
        {
            var baseRepo = new BaseRepositroy<TEntity>();
            return await baseRepo.Query();
        }
    }
}
