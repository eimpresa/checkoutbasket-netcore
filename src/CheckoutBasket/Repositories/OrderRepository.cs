using CheckoutBasket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Repositories
{
    public class OrderRepository : RepositoryBase<Order, Guid>, IOrderRepository
    {
        public OrderRepository() : base(x => x.Id)
        {
        }

        public Task AddItem(Order order, Product product, uint quantity)
        {
            order.Items.Add(new OrderItem { OrderId = order.Id, ProductId = product.Id, Quantity = quantity });
            return Task.CompletedTask;
        }

        public async Task DeleteItem(Guid orderId, Guid productId)
        {
            var order = (await GetById(orderId)) ?? throw new KeyNotFoundException(Messages.OrderNotFound);
            var item = order.Items.FirstOrDefault(x => x.ProductId == productId) ?? throw new KeyNotFoundException(string.Format(Messages.OrderDoesNotContainProduct, productId));
            order.Items.Remove(item);
        }
    }
}
