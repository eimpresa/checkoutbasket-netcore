using CheckoutBasket.Models;
using System;
using System.Threading.Tasks;

namespace CheckoutBasket.Repositories
{
    public interface IProductRepository : IRepositoryBase<Product, Guid>
    {
        Task ReserveItems(Guid productId, uint quantity);
        Task ReleaseItems(Guid productId, uint quantity);
    }
}