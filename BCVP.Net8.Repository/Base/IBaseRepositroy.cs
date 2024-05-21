﻿
using BCVP.Net8.Model;

namespace BCVP.Net8.Repository
{
    public interface IBaseRepositroy<TEntity> where TEntity : class
    {
        Task<List<TEntity>> Query();
    }
}
