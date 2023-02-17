using Bogus;
using FictiveShop.Core.Features.Basket;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.Requests;
using FictiveShop.Core.ValueObjects;
using FluentAssertions;
using Moq;
using System.Text.Json;

namespace FictiveShop.Tests.Basket_Tests
{
    [Trait("Get Basket", "Happy Path")]
    public class GetBasket_HappyPath
    {
        private readonly Mock<IInMemoryRedis> _mockRedis;
        private readonly GetBasketHandler _handler;

        public GetBasket_HappyPath()
        {
            _mockRedis = new Mock<IInMemoryRedis>();
            _handler = new GetBasketHandler(_mockRedis.Object);
        }

        [Fact]
        public async Task Handle_ValidQuery_ReturnsBasketResponse()
        {
            // Arrange
            var customerId = new Faker().Random.Guid().ToString();
            var customerBasket = new CustomerBasket
            {
                Items = new List<BasketItem>
                {
                    new BasketItem
                    {
                        ProductId = Guid.NewGuid().ToString(),
                        Quantity = 2,
                        Price = 100,
                        ProductName = "ProductA"
                    },
                    new BasketItem
                    {
                        ProductId = Guid.NewGuid().ToString(),
                        Quantity = 1,
                        Price = 100,
                        ProductName = "ProductB"
                    }
                }
            };
            _mockRedis.Setup(x => x.Get(customerId)).Returns(JsonSerializer.Serialize(customerBasket));

            var query = new BasketGetByCustomerIdRequest(customerId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEquivalentTo(customerBasket.Items);
            result.Items.Count.Should().Be(2);
        }
    }
}