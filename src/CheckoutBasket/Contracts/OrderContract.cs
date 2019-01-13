using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Contracts
{
    public class OrderContract
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public IEnumerable<OrderItemContract> Items { get; internal set; }
    }
}
