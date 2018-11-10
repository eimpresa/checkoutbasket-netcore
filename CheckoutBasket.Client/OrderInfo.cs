using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutBasket.Client
{
    public class OrderInfo
    {
        public Guid Id { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public IEnumerable<OrderItemInfo> Items { get; set; }
    }
}
