using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Contracts
{
    public class CreateProductContract
    {
        public string name { get; set; }
        public uint quantity { get; set; }
    }
}
