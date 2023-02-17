using FictiveShop.Core.Domain;
using FictiveShop.Core.Features.Orders;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.Requests;
using FictiveShop.Core.ValueObjects;
using Moq;
using System.Text.Json;

namespace FictiveShop.Tests.Order_Tests
{
    [Trait("Order Items", "Happy Path")]
    public class CreateOrder_HappyPath
    {
        private readonly Mock<IInMemoryRedis> _redisMock;
        private readonly Mock<IRepository<Order>> _orderRepoMock;
        private readonly Mock<IRepository<Product>> _productRepoMock;
        private readonly Mock<IRepository<Customer>> _customerRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IDateTimeProvider> _dateTimeProvider;

        private readonly CreateOrder _handler;

        public CreateOrder_HappyPath()
        {
            _redisMock = new();
            _orderRepoMock = new();
            _productRepoMock = new();
            _customerRepoMock = new();
            _unitOfWork = new();
            _dateTimeProvider = new();

            _handler = new CreateOrder(
               _unitOfWork.Object,
                _redisMock.Object,
                _orderRepoMock.Object,
                _productRepoMock.Object,
                _customerRepoMock.Object,
                _dateTimeProvider.Object
            );
        }

        [Fact]
        public async Task Should_Create_Order_Successfully()
        {
            // Arrange
            var customerId = Guid.NewGuid().ToString();
            var request = new OrderRequest(customerId, new AddressRequest("London", "11", "Street123"), "+44 123456789");
            _dateTimeProvider.SetReturnsDefault(new DateTime(2023, 2, 17, 20, 0, 0));

            var customerBasket = new CustomerBasket();
            customerBasket.Items.Add(new BasketItem
            {
                ProductId = Guid.NewGuid().ToString(),
                Quantity = 1,
                Price = 100,
                ProductName = "ProductA"
            });
            var basketJson = JsonSerializer.Serialize(customerBasket);
            _redisMock.Setup(r => r.Get(customerId)).Returns(basketJson).Verifiable();

            _productRepoMock.Setup(p => p.GetById(It.IsAny<string>()))
                .Returns(new Product
                {
                    Quantity = 10,
                    Name = "ProductA",
                    Price = new Price() { Amount = 100, Currency = "USD" }
                });
            _unitOfWork.SetReturnsDefault(true);

            _customerRepoMock.Setup(p => p.GetById(It.IsAny<string>()))
                .Returns(new Customer
                {
                    Address = new(),
                    Name = new() { FirstName = "John", LastName = "Doe" },
                    PhoneNumber = "0653331412"
                });

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.NotEqual(Guid.Empty.ToString(), response.OrderId);
            Assert.NotEqual(0m, response.TotalAmount);
            Assert.Equal(0m, response.AppliedDiscount);

            _redisMock.Verify();
            _orderRepoMock.Verify(r => r.Create(It.IsAny<Order>()), Times.Once);
            _productRepoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
            _customerRepoMock.Verify(r => r.Create(It.IsAny<Customer>()), Times.Once);
        }
    }
}