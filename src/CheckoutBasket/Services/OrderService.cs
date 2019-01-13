using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckoutBasket.Contracts;
using CheckoutBasket.Models;
using CheckoutBasket.Repositories;

namespace CheckoutBasket.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICheckoutBasketRepositories repos;

        public OrderService(ICheckoutBasketRepositories repos)
        {
            this.repos = repos;
        }

        public async Task<ServiceResponse> AddItem(Guid orderId, Guid productId, uint quantity)
        {
            var order = await repos.Orders.GetById(orderId);

            if (order == null)
            {
                return ServiceResponse.Fail(new[] { Messages.OrderNotFound });
            }

            var product = await repos.Products.GetById(productId);

            if (product == null)
            {
                return ServiceResponse.Fail(new[] { Messages.ProductNotFound });
            }

            var availableProducts = product.Quantity - product.Reserved;
            if (quantity > availableProducts)
            {
                return ServiceResponse.Fail(new[] { string.Format(Messages.ProductNotAvailableInThisQuantity, availableProducts) });
            }

            await repos.Products.ReserveItems(productId, quantity);
            await repos.Orders.AddItem(order, product, quantity);

            return ServiceResponse.Ok();
        }

        public async Task<ServiceResponse> ClearOrder(Guid orderId)
        {
            var order = await repos.Orders.GetById(orderId);

            if (order == null)
            {
                return ServiceResponse.Fail(new[] { Messages.OrderNotFound });
            }

            foreach (var item in order.Items)
            {
                await repos.Products.ReleaseItems(item.ProductId, item.Quantity);
                await repos.Orders.DeleteItem(orderId, item.ProductId);
            }

            return ServiceResponse.Ok();
        }

        public async Task<ServiceResponse<OrderCreatedContract>> CreateOrder(Guid userId)
        {
            var order = new Order
            {
                DateCreatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Items = new List<OrderItem>(),
                UserId = userId
            };

            await repos.Orders.Add(order);

            return ServiceResponse.Ok(new OrderCreatedContract { Id = order.Id });
        }

        public async Task<ServiceResponse> DeleteItem(Guid orderId, Guid productId)
        {
            var order = await repos.Orders.GetById(orderId);

            if (order == null)
            {
                return ServiceResponse.Fail(new[] { Messages.OrderNotFound });
            }

            var item = order.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
            {
                return ServiceResponse.Fail(new[] { string.Format(Messages.OrderDoesNotContainProduct, productId) });
            }

            await repos.Products.ReleaseItems(productId, item.Quantity);
            await repos.Orders.DeleteItem(orderId, productId);

            return ServiceResponse.Ok();
        }

        public async Task<OrderContract> GetOrder(Guid id)
        {
            var order = await repos.Orders.GetById(id);
            if (order != null)
            {
                return new OrderContract
                {
                    DateCreatedUtc = order.DateCreatedUtc,
                    Id = order.Id,
                    Items = order.Items.Select(x => new OrderItemContract { ProductId = x.ProductId, Quantity = x.Quantity }),
                    UserId = order.UserId
                };
            }

            return null;
        }

        public async Task<ServiceResponse> UpdateQuantity(Guid orderId, Guid productId, uint newQuantity)
        {
            var order = await repos.Orders.GetById(orderId);

            if (order == null)
            {
                return ServiceResponse.Fail(new[] { Messages.OrderNotFound });
            }

            var product = await repos.Products.GetById(productId) ?? throw new InvalidOperationException(Messages.ProductNotFound);
            var item = order.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
            {
                return ServiceResponse.Fail(new[] { string.Format(Messages.OrderDoesNotContainProduct, productId) });
            }

            if (newQuantity > item.Quantity)
            {
                var availableProducts = product.Quantity - product.Reserved;
                if (availableProducts < newQuantity - item.Quantity)
                {
                    return ServiceResponse.Fail(new[] { string.Format(Messages.ProductNotAvailableInThisQuantity, availableProducts) });
                }

                await repos.Products.ReserveItems(productId, newQuantity - item.Quantity);
                item.Quantity = newQuantity;
            }
            else if (newQuantity < item.Quantity)
            {
                await repos.Products.ReleaseItems(productId, item.Quantity - newQuantity);
                item.Quantity = newQuantity;
            }

            return ServiceResponse.Ok();
        }
    }
}
