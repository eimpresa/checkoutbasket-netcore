using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutBasket.Client
{
    public class OrderItemInfo
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
    }
}
