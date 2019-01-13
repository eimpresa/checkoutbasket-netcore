using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Contracts
{
    public class OrderItemContract
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
    }
}
