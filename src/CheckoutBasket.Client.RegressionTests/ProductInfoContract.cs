using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.RegressionTests
{
    public class ProductInfoContract
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public uint quantity { get; set; }
        public uint reserved { get; set; }
    }
}
