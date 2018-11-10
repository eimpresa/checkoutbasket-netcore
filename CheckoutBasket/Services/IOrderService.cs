using CheckoutBasket.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Services
{
    public interface IOrderService
    {
        Task<ServiceResponse> AddItem(Guid orderId, Guid productId, uint quantity);
        Task<ServiceResponse> DeleteItem(Guid orderId, Guid productId);
        Task<ServiceResponse> UpdateQuantity(Guid orderId, Guid productId, uint newQuantity);
        Task<ServiceResponse> ClearOrder(Guid id);
        Task<ServiceResponse<OrderCreatedContract>> CreateOrder(Guid userId);
        Task<OrderContract> GetOrder(Guid id);
    }
}
