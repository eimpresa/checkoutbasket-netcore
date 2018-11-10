using CheckoutBasket.Client;
using System;
using System.Linq;
using System.Net;
using Xunit;

namespace CheckoutBasket.RegressionTests
{
    public class CheckoutBasketClientTests
    {
        private const string BaseUrl = "http://localhost:55687/api/v1/";
        private readonly CheckoutBasketClient client;
        private readonly TestDataFactory testData;

        public CheckoutBasketClientTests()
        {
            var adminClient = new CheckoutBasketTestClient(
                BaseUrl,
                new NetworkCredential("pRpsUKcFzxzupxGyeaPyHx2nN6NBos2iD7peRXme", "Fu6fswGfmgbg9fNA68vmjzUSY2dYK5WtsoA8RyXn"));
            this.client = new CheckoutBasketClient(
                BaseUrl,
                new NetworkCredential("bqEEg3c2HLM4jMXLvxDBboQzYGn2JuNPoL7tDSV4", "vQwJgfbNGWmEj5WMxy3ZPCJKZxTYvBCCTJzekBd3"));
            this.testData = new TestDataFactory(adminClient);
        }

        private Guid AssertOrderCreated(Client.ServiceResponse<OrderCreated> res)
        {
            Assert.NotNull(res);
            Assert.True(res.Ok);
            Assert.Null(res.Errors);
            Assert.NotNull(res.Data);
            return res.Data.Id;
        }

        [Fact]
        public async void ShouldCreateAndRetrieveOrder()
        {
            // arrange

            // act
            var res = await client.CreateOrder();

            // assert
            var orderId = AssertOrderCreated(res);
            var order = await client.GetOrder(res.Data.Id);
            Assert.NotNull(order);
            Assert.Equal(res.Data.Id, order.Id);
            Assert.True(order.DateCreatedUtc > DateTime.MinValue);
            Assert.NotNull(order.Items);
        }

        [Fact]
        public async void ShouldAddMultipleItemsToOrderAndReserveProducts()
        {
            // arrange
            const uint P1Reservation = 5;
            const uint P2Reservation = 55;
            var p1 = await testData.CreateProduct(10);
            var p2 = await testData.CreateProduct(100);

            // act
            var createOrderRes = await client.CreateOrder();
            var orderId = AssertOrderCreated(createOrderRes);
            var addItem1Res = await client.AddItem(orderId, p1.id, P1Reservation);
            var addItem2Res = await client.AddItem(orderId, p2.id, P2Reservation);

            // assert
            Assert.True(addItem1Res.Ok);
            Assert.True(addItem2Res.Ok);
            var order = await client.GetOrder(orderId);
            Assert.NotNull(order);
            Assert.NotNull(order.Items);
            Assert.Collection(
                order.Items,
                item1 => { Assert.Equal(p1.id, item1.ProductId); Assert.Equal(P1Reservation, item1.Quantity); },
                item2 => { Assert.Equal(p2.id, item2.ProductId); Assert.Equal(P2Reservation, item2.Quantity); });
            p1 = await testData.GetProduct(p1.id);
            p2 = await testData.GetProduct(p2.id);
            Assert.Equal(P1Reservation, p1.reserved);
            Assert.Equal(P2Reservation, p2.reserved);
        }

        [Theory]
        [InlineData(1, 9)]
        [InlineData(9, 1)]
        [InlineData(9, 9)]
        public async void ShouldUpdateItemQuantityAndProductReservations(uint initialQuantity, uint expectedQuantity)
        {
            // arrange
            var p1 = await testData.CreateProduct(10);

            // act
            var createOrderRes = await client.CreateOrder();
            var orderId = AssertOrderCreated(createOrderRes);
            var addItem1Res = await client.AddItem(orderId, p1.id, initialQuantity);

            // assert
            Assert.True(addItem1Res.Ok);
            var order = await client.GetOrder(orderId);
            Assert.NotNull(order);
            Assert.NotNull(order.Items);
            Assert.Single(order.Items);

            // act
            var updateItem1Res = await client.UpdateItem(orderId, p1.id, expectedQuantity);

            // assert
            Assert.True(addItem1Res.Ok);
            order = await client.GetOrder(orderId);
            Assert.NotNull(order);
            Assert.NotNull(order.Items);
            Assert.Single(order.Items);
            Assert.Equal(p1.id, order.Items.First().ProductId);
            Assert.Equal(expectedQuantity, order.Items.First().Quantity);
            p1 = await testData.GetProduct(p1.id);
            Assert.Equal(expectedQuantity, p1.reserved);
        }

        [Fact]
        public async void ShouldDeleteItemsFromOrderAndReleaseProductReservations()
        {
            // arrange
            var p1 = await testData.CreateProduct(10);
            var p2 = await testData.CreateProduct(100);

            // act
            var createOrderRes = await client.CreateOrder();
            var orderId = AssertOrderCreated(createOrderRes);
            var addItem1Res = await client.AddItem(orderId, p1.id, 9);
            var addItem2Res = await client.AddItem(orderId, p2.id, 99);
            var order = await client.GetOrder(orderId);
            Assert.NotNull(order);
            Assert.NotNull(order.Items);
            Assert.Equal(2, order.Items.Count());
            await client.DeleteItem(orderId, p1.id);
            await client.DeleteItem(orderId, p2.id);

            // assert
            order = await client.GetOrder(orderId);
            Assert.NotNull(order);
            Assert.NotNull(order.Items);
            Assert.Empty(order.Items);
            p1 = await testData.GetProduct(p1.id);
            p2 = await testData.GetProduct(p2.id);
            Assert.Equal((uint)0, p1.reserved);
            Assert.Equal((uint)0, p2.reserved);
        }

        [Fact]
        public async void ShouldClearOrderAndReleaseProductReservations()
        {
            // arrange
            var p1 = await testData.CreateProduct(10);
            var p2 = await testData.CreateProduct(100);

            // act
            var createOrderRes = await client.CreateOrder();
            var orderId = AssertOrderCreated(createOrderRes);
            var addItem1Res = await client.AddItem(orderId, p1.id, 9);
            var addItem2Res = await client.AddItem(orderId, p2.id, 99);
            var order = await client.GetOrder(orderId);
            Assert.NotNull(order);
            Assert.NotNull(order.Items);
            Assert.Equal(2, order.Items.Count());
            await client.ClearOrder(orderId);

            // assert
            order = await client.GetOrder(orderId);
            Assert.NotNull(order);
            Assert.NotNull(order.Items);
            Assert.Empty(order.Items);
            p1 = await testData.GetProduct(p1.id);
            p2 = await testData.GetProduct(p2.id);
            Assert.Equal((uint)0, p1.reserved);
            Assert.Equal((uint)0, p2.reserved);
        }
    }
}
