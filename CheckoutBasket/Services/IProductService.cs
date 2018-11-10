using System;
using System.Threading.Tasks;
using CheckoutBasket.Contracts;

namespace CheckoutBasket.Services
{
    public interface IProductService
    {
        Task<ServiceResponse<ProductCreatedContract>> CreateProduct(CreateProductContract product);
        Task<ProductInfoContract> GetProduct(Guid productId);
    }
}