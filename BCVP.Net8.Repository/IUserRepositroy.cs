
using BCVP.Net8.Model;

namespace BCVP.Net8.Repository
{
    public interface IUserRepositroy
    {
        Task<List<User>> Query();
    }
}
