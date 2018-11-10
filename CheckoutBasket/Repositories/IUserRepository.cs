using CheckoutBasket.Models;
using System;
using System.Threading.Tasks;

namespace CheckoutBasket.Repositories
{
    public interface IUserRepository : IRepositoryBase<User, Guid>
    {
        Task<User> GetUserByCredentials(string username, string password);
    }
}