using BCVP.Net8.Model;
using Newtonsoft.Json;

namespace BCVP.Net8.Repository
{
    public class UserRepositroy : IUserRepositroy
    {
        public async Task<List<User>> Query()
        {
            await Task.CompletedTask;
            var data = "[{\"Id\":18,\"Name\":\"Jeff\"}]";

            return JsonConvert.DeserializeObject<List<User>>(data) ?? new List<User>();
        }
    }
}
