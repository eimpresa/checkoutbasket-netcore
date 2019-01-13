using System;

namespace CheckoutBasket.Models
{
    public class OrderItem
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
    }
}
