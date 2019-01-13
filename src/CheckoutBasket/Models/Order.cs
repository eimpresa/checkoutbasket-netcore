using System;
using System.Collections.Generic;

namespace CheckoutBasket.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public ICollection<OrderItem> Items { get; set; }
        public Guid UserId { get; set; }
    }
}
