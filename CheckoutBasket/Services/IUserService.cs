using CheckoutBasket.Contracts;
using CheckoutBasket.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Services
{
    public interface IUserService
    {
        Task<UserInfo> GetUserByCredentials(string username, string password);
    }
}
