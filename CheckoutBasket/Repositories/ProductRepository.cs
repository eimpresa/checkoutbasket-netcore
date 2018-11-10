using CheckoutBasket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Repositories
{
    public class ProductRepository : RepositoryBase<Product, Guid>, IProductRepository
    {
        public ProductRepository() : base(x => x.Id)
        {
            Add(new Product { Id = Guid.Parse("081c4429-6763-4f8b-8627-748cf32e1f9a"), Name = "Olive oil", Quantity = 2, Reserved = 0 });
            Add(new Product { Id = Guid.Parse("9bec962f-599d-4f69-b9d2-f692938ba4fa"), Name = "Extra virgin Olive oil", Quantity = 1, Reserved = 0 });
        }

        public async Task ReleaseItems(Guid productId, uint quantity)
        {
            var product = (await GetById(productId)) ?? throw new KeyNotFoundException(Messages.ProductNotFound);
            product.Reserved -= quantity;
        }

        public async Task ReserveItems(Guid productId, uint quantity)
        {
            var product = (await GetById(productId)) ?? throw new KeyNotFoundException(Messages.ProductNotFound);
            product.Reserved += quantity;
        }
    }
}
