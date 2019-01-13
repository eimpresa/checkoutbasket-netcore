using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CheckoutBasket.RegressionTests
{
    public class TestDataFactory
    {
        private readonly CheckoutBasketTestClient client;

        public TestDataFactory(CheckoutBasketTestClient client)
        {
            this.client = client;
        }

        public async Task<ProductInfoContract> CreateProduct(uint quantity)
        {
            var createProductRes = await client.CreateProduct(Guid.NewGuid().ToString(), quantity);
            Assert.True(createProductRes.Success);
            var getProductRes = await client.GetProduct(createProductRes.Data);
            Assert.NotNull(getProductRes);
            return getProductRes;
        }

        public async Task<ProductInfoContract> GetProduct(Guid productId)
        {
            var res = await client.GetProduct(productId);
            Assert.NotNull(res);
            return res;
        }
    }
}
