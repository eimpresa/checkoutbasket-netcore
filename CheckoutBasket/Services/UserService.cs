using CheckoutBasket.Contracts;
using CheckoutBasket.Repositories;
using CheckoutBasket.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Services
{
    public class UserService : IUserService
    {
        private readonly ICheckoutBasketRepositories repos;

        public UserService(ICheckoutBasketRepositories repos)
        {
            this.repos = repos;
        }

        public async Task<UserInfo> GetUserByCredentials(string username, string password)
        {
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                var user = await repos.Users.GetUserByCredentials(username, password);
                if (user != null)
                {
                    return new UserInfo { Id = user.Id, Role = user.Role };
                }
            }

            return null;
        }
    }
}
