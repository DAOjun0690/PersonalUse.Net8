using AutoMapper;
using BCVP.Net8.IService;
using BCVP.Net8.Model;
using BCVP.Net8.Repository;

namespace BCVP.Net8.Service
{
    public class BaseService<TEntity, TVo> : 
        IBaseService<TEntity, TVo> where TEntity : class, new()
    {
        private readonly IMapper _mapper;
        private readonly IBaseRepositroy<TEntity> _baseRepositroy;

        public BaseService(IMapper mapper, IBaseRepositroy<TEntity> baseRepositroy)
        {
            _mapper = mapper;
            _baseRepositroy = baseRepositroy;
        }

        public async Task<List<TVo>> Query()
        {
            var entities = await _baseRepositroy.Query();
            Console.WriteLine($"_baseRepositroy 實例HashCode:  {_baseRepositroy.GetHashCode()}");
            var llout = _mapper.Map<List<TVo>>(entities);
            return llout;
        }
    }
}
