using Bogus;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Features.Basket;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.Requests;
using FictiveShop.Core.ValueObjects;
using FictiveShop.Infrastructure.DataAccess;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace FictiveShop.Tests.Basket_Tests
{
    [Trait("Update Basket", "Happy Path")]
    public class UpdateBasket_HappyPath
    {
        private readonly FictiveShopDbContext _dbContext;
        private readonly Mock<IInMemoryRedis> _redisMock;
        private readonly Mock<IBasketService> _basketServiceMock;
        private readonly Mock<IHostEnvironment> _env;
        private readonly Mock<IRepository<Product>> _productsRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ILogger<AddOrUpdateBasket>> _logger;

        public UpdateBasket_HappyPath()
        {
            _dbContext = new();
            _redisMock = new();
            _basketServiceMock = new();
            _env = new();
            _productsRepoMock = new();
            _unitOfWork = new();
            _logger = new();
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

            var request = new Faker<BasketUpdateRequest>()
                         .CustomInstantiator(f => new BasketUpdateRequest(
                         f.Random.Guid().ToString(),
                         product.Id,
                         f.Random.Int(1, 5))).Generate();

            _dbContext.Products.Add(product);
            _productsRepoMock.Setup(r => r.GetById(product.Id)).Returns(product);
            _redisMock.Setup(r => r.Get(request.CustomerId)).Returns(JsonSerializer.Serialize(customerBasket)).Verifiable();
            _basketServiceMock.Setup(s => s.UpdateBasket(It.IsAny<CustomerBasket>(), request, product)).Returns(true);
            _env.Setup(r => r.EnvironmentName).Returns("Test");
            _unitOfWork.SetReturnsDefault(true);

            var handler = new AddOrUpdateBasket(_productsRepoMock.Object, _redisMock.Object, _basketServiceMock.Object, _env.Object, _unitOfWork.Object, _logger.Object);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            _redisMock.Verify(r => r.Get(request.CustomerId), Times.AtLeast(1));
            Assert.True(response.IsBasketUpdated);
        }
    }
}