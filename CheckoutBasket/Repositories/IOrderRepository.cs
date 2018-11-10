using CheckoutBasket.Models;
using System;
using System.Threading.Tasks;

namespace CheckoutBasket.Repositories
{
    public interface IOrderRepository : IRepositoryBase<Order, Guid>
    {
        Task AddItem(Order order, Product product, uint quantity);
        Task DeleteItem(Guid orderId, Guid productId);
    }
}