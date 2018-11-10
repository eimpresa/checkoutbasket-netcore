using CheckoutBasket.Contracts;
using CheckoutBasket.Models;
using CheckoutBasket.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Services
{
    public class ProductService : IProductService
    {
        private readonly ICheckoutBasketRepositories repos;

        public ProductService(ICheckoutBasketRepositories repos)
        {
            this.repos = repos;
        }

        public async Task<ServiceResponse<ProductCreatedContract>> CreateProduct(CreateProductContract product)
        {
            var newProduct = new Product { Id = Guid.NewGuid(), Name = product.name, Quantity = product.quantity };
            await repos.Products.Add(newProduct);
            return ServiceResponse.Ok(new ProductCreatedContract { id = newProduct.Id });
        }

        public async Task<ProductInfoContract> GetProduct(Guid productId)
        {
            var product = await repos.Products.GetById(productId);
            if (product != null)
            {
                return new ProductInfoContract { id = productId, name = product.Name, quantity = product.Quantity, reserved = product.Reserved };
            }

            return null;
        }
    }
}
