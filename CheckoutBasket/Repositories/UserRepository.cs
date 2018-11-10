using CheckoutBasket.Constants;
using CheckoutBasket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Repositories
{
    public class UserRepository : RepositoryBase<User, Guid>, IUserRepository
    {
        public UserRepository() : base(x => x.Id)
        {
            Add(new User { Id = Guid.NewGuid(), Username = "pRpsUKcFzxzupxGyeaPyHx2nN6NBos2iD7peRXme", Password = "Fu6fswGfmgbg9fNA68vmjzUSY2dYK5WtsoA8RyXn", Role = UserRoles.Admin });
            Add(new User { Id = Guid.NewGuid(), Username = "bqEEg3c2HLM4jMXLvxDBboQzYGn2JuNPoL7tDSV4", Password = "vQwJgfbNGWmEj5WMxy3ZPCJKZxTYvBCCTJzekBd3", Role = UserRoles.BasketUser });
        }

        public Task<User> GetUserByCredentials(string username, string password)
        {
            return Task.FromResult(Data.Values.FirstOrDefault(x => string.Equals(username, x.Username, StringComparison.OrdinalIgnoreCase) && password == x.Password));
        }
    }
}
