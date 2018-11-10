using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Repositories
{
    public class CheckoutBasketRepositories : ICheckoutBasketRepositories
    {
        private IOrderRepository orders;
        private IProductRepository products;
        private IUserRepository users;

        public CheckoutBasketRepositories()
        {
        }

        public IOrderRepository Orders => orders ?? (orders = new OrderRepository());
        public IProductRepository Products => products ?? (products = new ProductRepository());
        public IUserRepository Users => users ?? (users = new UserRepository());
    }
}
