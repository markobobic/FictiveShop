using Bogus;
using FictiveShop.Api.Features.Basket;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Dtos;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
using FictiveShop.Infrastructure.DataAccess;
using Moq;
using System.Text.Json;

namespace FictiveShop.Tests.Basket
{
    public class AddOrUpdateBasketTests
    {
        private readonly FictiveShopDbContext _dbContext;
        private readonly Mock<IInMemoryRedis> _redisMock;
        private readonly Mock<IBasketService> _basketServiceMock;
        private readonly Mock<ISupplierStockService> _supplierStockService;

        public AddOrUpdateBasketTests()
        {
            _dbContext = new FictiveShopDbContext();
            _redisMock = new Mock<IInMemoryRedis>();
            _basketServiceMock = new Mock<IBasketService>();
            _supplierStockService = new Mock<ISupplierStockService>();
        }

        [Fact]
        public async Task Handle_WhenProductAndBasketExist_UpdatesBasket()
        {
            // Arrange

            var priceFaker = new Faker<Price>()
            .RuleFor(p => p.Amount, f => f.Finance.Amount())
            .RuleFor(p => p.Currency, f => f.Finance.Currency().Symbol);

            var product = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Guid().ToString())
                .RuleFor(p => p.Quantity, f => f.Random.Int(10, 100))
                .RuleFor(p => p.Name, f => f.Commerce.Product())
                .RuleFor(p => p.Price, f => priceFaker.Generate())
                .Generate();

            var customerBasket = new Faker<CustomerBasket>()
                .RuleFor(b => b.Items, f => new List<BasketItem> { new BasketItem { Price = f.Finance.Amount(), ProductId = product.Id, ProductName = product.Name, Quantity = product.Quantity } })
                .Generate();

            var command = new Faker<AddOrUpdateBasket.Command>()
                .RuleFor(c => c.Request, f => new BasketUpdateDto { CustomerId = f.Random.Guid().ToString(), ProductId = product.Id, Quantity = f.Random.Int(1, 5) })
                .Generate();

            _dbContext.Products.Add(product);
            _redisMock.Setup(r => r.Get(command.Request.CustomerId)).Returns(JsonSerializer.Serialize(customerBasket));
            _basketServiceMock.Setup(s => s.UpdateBasket(customerBasket, command.Request, product)).Returns(true);
            _supplierStockService.Setup(s => s.IsAvailableInStock(product.Id, command.Request.Quantity))
                    .ReturnsAsync(true);

            var handler = new AddOrUpdateBasket.Handler(_dbContext, _redisMock.Object, _basketServiceMock.Object, _supplierStockService.Object, _);

            // Act
            var response = await handler.Handle(command, CancellationToken.None);

            // Assert
            _redisMock.Verify(r => r.Get(command.Request.CustomerId), Times.Once);
            _basketServiceMock.Verify(s => s.UpdateBasket(customerBasket, command.Request, product), Times.Once);
            Assert.True(response.IsBasketUpdated);
        }
    }
}