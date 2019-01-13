using CheckoutBasket.Models;
using CheckoutBasket.Repositories;
using CheckoutBasket.Services;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CheckoutBasket.UnitTests.Services
{
    public class OrderServiceTests : IDisposable
    {
        private readonly Mock<ICheckoutBasketRepositories> mockRepos;
        private readonly OrderService orderService;

        public OrderServiceTests()
        {
            this.mockRepos = new Mock<ICheckoutBasketRepositories>(MockBehavior.Strict);
            this.orderService = new OrderService(mockRepos.Object);
        }

        [Fact]
        public async void ShouldAddItemWhenProductExistsAndIsAvailable()
        {
            // arrange
            const uint Quantity = 99;
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var order = new Order();
            var product = new Product { Quantity = Quantity + 1 };

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(order)).Verifiable();
            mockRepos.Setup(x => x.Products.GetById(productId)).Returns(Task.FromResult(product)).Verifiable();
            mockRepos.Setup(x => x.Products.ReserveItems(productId, Quantity)).Returns(Task.CompletedTask).Verifiable();
            mockRepos.Setup(x => x.Orders.AddItem(order, product, Quantity)).Returns(Task.CompletedTask).Verifiable();

            // act
            var res = await orderService.AddItem(orderId, productId, Quantity);

            // assert
            Assert.NotNull(res);
            Assert.True(res.Success);
            Assert.Null(res.Errors);
        }

        [Fact]
        public async void ShouldNotAddItemWhenOrderDoesNotExist()
        {
            // arrange
            const uint Quantity = 99;
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(null as Order)).Verifiable();

            // act
            var res = await orderService.AddItem(orderId, productId, Quantity);

            // assert
            Assert.NotNull(res);
            Assert.False(res.Success);
            Assert.Contains(Messages.OrderNotFound, res.Errors);
        }

        [Fact]
        public async void ShouldNotAddItemWhenProductDoesNotExist()
        {
            // arrange
            const uint Quantity = 99;
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var order = new Order();

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(order)).Verifiable();
            mockRepos.Setup(x => x.Products.GetById(productId)).Returns(Task.FromResult(null as Product)).Verifiable();

            // act
            var res = await orderService.AddItem(orderId, productId, Quantity);

            // assert
            Assert.NotNull(res);
            Assert.False(res.Success);
            Assert.Contains(Messages.ProductNotFound, res.Errors);
        }

        [Fact]
        public async void ShouldNotAddItemWhenProductIsNotAvailableInTheSpecifiedQuantity()
        {
            // arrange
            const uint Quantity = 99;
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var order = new Order();
            var product = new Product { Quantity = Quantity - 1 };

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(order)).Verifiable();
            mockRepos.Setup(x => x.Products.GetById(productId)).Returns(Task.FromResult(product)).Verifiable();

            // act
            var res = await orderService.AddItem(orderId, productId, Quantity);

            // assert
            Assert.NotNull(res);
            Assert.False(res.Success);
            Assert.Contains(string.Format(Messages.ProductNotAvailableInThisQuantity, Quantity - 1), res.Errors);
        }

        [Fact]
        public async void ShouldDeleteExistingItemFromOrderAndReleaseProductReservations()
        {
            // arrange
            var orderId = Guid.NewGuid();
            var p1Id = Guid.NewGuid();
            var p2Id = Guid.NewGuid();
            uint p1Quantity = 99;
            uint p2Quantity = 100;
            var order = new Order { Items = new[] { new OrderItem { ProductId = p1Id, Quantity = p1Quantity }, new OrderItem { ProductId = p2Id, Quantity = p2Quantity } } };

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(order)).Verifiable();
            mockRepos.Setup(x => x.Products.ReleaseItems(p2Id, p2Quantity)).Returns(Task.CompletedTask).Verifiable();
            mockRepos.Setup(x => x.Orders.DeleteItem(orderId, p2Id)).Returns(Task.CompletedTask).Verifiable();

            // act
            var res = await orderService.DeleteItem(orderId, p2Id);

            // assert
            Assert.NotNull(res);
            Assert.True(res.Success);
            Assert.Null(res.Errors);
        }

        [Fact]
        public async void ShouldNotDeleteItemWhenOrderDoesNotExist()
        {
            // arrange
            var orderId = Guid.NewGuid();
            var p1Id = Guid.NewGuid();

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(null as Order)).Verifiable();

            // act
            var res = await orderService.DeleteItem(orderId, p1Id);

            // assert
            Assert.NotNull(res);
            Assert.False(res.Success);
            Assert.Contains(Messages.OrderNotFound, res.Errors);
        }

        [Fact]
        public async void ShouldNotDeleteItemWhenOrderDoesNotContainProduct()
        {
            // arrange
            var orderId = Guid.NewGuid();
            var p1Id = Guid.NewGuid();

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(new Order { Items = new OrderItem[0] })).Verifiable();

            // act
            var res = await orderService.DeleteItem(orderId, p1Id);

            // assert
            Assert.NotNull(res);
            Assert.False(res.Success);
            Assert.Contains(string.Format(Messages.OrderDoesNotContainProduct, p1Id), res.Errors);
        }

        [Theory]
        [InlineData(99)]
        [InlineData(100)]
        [InlineData(101)]
        public async void ShouldUpdateItemQuantityWhenProductIsAvailable(uint newQuantity)
        {
            // arrange
            const uint Quantity = 100;
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var order = new Order { Items = new[] { new OrderItem { ProductId = productId, Quantity = Quantity } } };
            var product = new Product { Quantity = 102 };

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(order)).Verifiable();
            mockRepos.Setup(x => x.Products.GetById(productId)).Returns(Task.FromResult(product)).Verifiable();
            if (newQuantity > Quantity)
            {
                mockRepos.Setup(x => x.Products.ReserveItems(productId, newQuantity - Quantity)).Returns(Task.CompletedTask).Verifiable();
            }
            else if (newQuantity < Quantity)
            {
                mockRepos.Setup(x => x.Products.ReleaseItems(productId, Quantity - newQuantity)).Returns(Task.CompletedTask).Verifiable();
            }

            // act
            var res = await orderService.UpdateQuantity(orderId, productId, newQuantity);

            // assert
            Assert.NotNull(res);
            Assert.True(res.Success);
            Assert.Null(res.Errors);
        }

        [Fact]
        public async void ShouldNotUpdateItemQuantityWhenOrderDoesNotExist()
        {
            // arrange
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(null as Order)).Verifiable();

            // act
            var res = await orderService.UpdateQuantity(orderId, productId, 1);

            // assert
            Assert.NotNull(res);
            Assert.False(res.Success);
            Assert.Contains(Messages.OrderNotFound, res.Errors);
        }

        [Fact]
        public async void ShouldThrowWhenTryingToUpdateQuantityForNonExistingProduct()
        {
            // arrange
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(new Order { Items = new OrderItem[0] })).Verifiable();
            mockRepos.Setup(x => x.Products.GetById(productId)).Returns(Task.FromResult(null as Product)).Verifiable();

            // act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => orderService.UpdateQuantity(orderId, productId, 1));

            // assert
            Assert.Equal(Messages.ProductNotFound, ex.Message);
        }

        [Fact]
        public async void ShouldNotUpdateItemQuantityWhenNewQuantityIsNotAvailable()
        {
            // arrange
            const uint NewQuantity = 101;
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var order = new Order { Items = new[] { new OrderItem { ProductId = productId, Quantity = 99 } } };
            var product = new Product { Quantity = 100, Reserved = 99 };

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(order)).Verifiable();
            mockRepos.Setup(x => x.Products.GetById(productId)).Returns(Task.FromResult(product)).Verifiable();

            // act
            var res = await orderService.UpdateQuantity(orderId, productId, NewQuantity);

            // assert
            Assert.NotNull(res);
            Assert.False(res.Success);
            Assert.Contains(string.Format(Messages.ProductNotAvailableInThisQuantity, 1), res.Errors);
        }

        [Fact]
        public async void ShouldClearOrderWithMultipleItemsAndReleaseProductReservation()
        {
            // arrange
            var p1Id = Guid.NewGuid();
            var p2Id = Guid.NewGuid();
            uint p1Quantity = 100;
            uint p2Quantity = 101;
            var orderId = new Guid();
            var order = new Order { Items = new[] { new OrderItem { ProductId = p1Id, Quantity = p1Quantity }, new OrderItem { ProductId = p2Id, Quantity = p2Quantity } } };

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(order)).Verifiable();
            mockRepos.Setup(x => x.Products.ReleaseItems(p1Id, p1Quantity)).Returns(Task.CompletedTask).Verifiable();
            mockRepos.Setup(x => x.Products.ReleaseItems(p2Id, p2Quantity)).Returns(Task.CompletedTask).Verifiable();
            mockRepos.Setup(x => x.Orders.DeleteItem(orderId, p1Id)).Returns(Task.CompletedTask).Verifiable();
            mockRepos.Setup(x => x.Orders.DeleteItem(orderId, p2Id)).Returns(Task.CompletedTask).Verifiable();

            // act
            var res = await orderService.ClearOrder(orderId);

            // assert
            Assert.NotNull(res);
            Assert.True(res.Success);
            Assert.Null(res.Errors);
        }

        [Fact]
        public async void ShouldNotClearOrderWhenOrderDoesNotExist()
        {
            // arrange
            var orderId = new Guid();

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(null as Order)).Verifiable();

            // act
            var res = await orderService.ClearOrder(orderId);

            // assert
            Assert.NotNull(res);
            Assert.False(res.Success);
            Assert.Contains(Messages.OrderNotFound, res.Errors);
        }

        [Fact]
        public async void ShouldCreateOrder()
        {
            // arrange
            var userId = Guid.NewGuid();
            mockRepos.Setup(x => x.Orders.Add(It.Is<Order>(y => y.UserId == userId))).Returns(Task.CompletedTask).Verifiable();

            // act
            var res = await orderService.CreateOrder(userId);

            // assert
            Assert.NotNull(res);
            Assert.True(res.Success);
            Assert.Null(res.Errors);
            Assert.NotNull(res.Data);
        }

        [Fact]
        public async void ShouldGetOrderById() {
            // arrange
            var dateCreatedUtc = DateTime.UtcNow;
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = new Order { DateCreatedUtc = dateCreatedUtc, Id = orderId, Items = new OrderItem[0], UserId = userId  };

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(order)).Verifiable();

            // act 
            var orderInfo = await orderService.GetOrder(orderId);

            // assert
            Assert.NotNull(orderInfo);
            Assert.Equal(dateCreatedUtc, orderInfo.DateCreatedUtc);
            Assert.Equal(orderId, orderInfo.Id);
            Assert.NotNull(orderInfo.Items);
            Assert.Equal(userId, orderInfo.UserId);
        }

        [Fact]
        public async void ShouldNotGetOrderByIdWhenOrderDoesNotExist()
        {
            // arrange
            var orderId = Guid.NewGuid();

            mockRepos.Setup(x => x.Orders.GetById(orderId)).Returns(Task.FromResult(null as Order)).Verifiable();

            // act 
            var orderInfo = await orderService.GetOrder(orderId);

            // assert
            Assert.Null(orderInfo);
        }

        public void Dispose()
        {
            mockRepos.Verify();
        }
    }
}
