using Ardalis.GuardClauses;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.Requests;
using FictiveShop.Core.Responses;
using FictiveShop.Core.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace FictiveShop.Core.Features.Basket
{
    public class AddOrUpdateBasket : ICommandHandler<BasketUpdateRequest, BasketUpdateResponse>
    {
        private readonly IRepository<Product> _productsRepository;
        private readonly IInMemoryRedis _redisDb;
        private readonly IBasketService _basketService;
        private readonly IHostEnvironment _env;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddOrUpdateBasket> _logger;
        private readonly Mock<ISupplierStockService> _mockSupplierStockService;

        public AddOrUpdateBasket(IRepository<Product> productsRepository,
                       IInMemoryRedis redisDb,
                       IBasketService basketService,
                       IHostEnvironment env,
                       IUnitOfWork unitOfWork,
                       ILogger<AddOrUpdateBasket> logger = null)
        {
            _productsRepository = productsRepository;
            _redisDb = redisDb;
            _basketService = basketService;
            _mockSupplierStockService = new Mock<ISupplierStockService>();
            _env = env;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public class CommandValidator : AbstractValidator<BasketUpdateRequest>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ProductId).NotNull().NotEmpty();
                RuleFor(x => x.Quantity).GreaterThan(0);
            }
        }

        public async Task<BasketUpdateResponse> Handle(BasketUpdateRequest request, CancellationToken cancellationToken)
        {
            bool enoughItemsInStock = true, isUpdated = false;
            var product = _productsRepository.GetById(request.ProductId);

            Guard.Against.Null(product, nameof(product), $"Product with ID: {request.ProductId} doens't exist");
            if (request.Quantity > product.Quantity)
            {
                enoughItemsInStock = await CallToExternalStock(request, enoughItemsInStock, product);
            }

            Guard.Against.OutOfStock(enoughItemsInStock);

            if (_redisDb.Get(request.CustomerId) is null)
            {
                var newBasket = new CustomerBasket();
                isUpdated = _basketService.AddToBasket(newBasket, request, product);

                await _unitOfWork.SaveChangesAsync();
                return new BasketUpdateResponse(isUpdated);
            }

            var basketData = _redisDb.Get(request.CustomerId);
            var existingBasket = JsonSerializer.Deserialize<CustomerBasket>(basketData);
            var existingProduct = existingBasket.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            var existingProductQuantity = existingProduct?.Quantity ?? 0;
            if (existingProductQuantity + request.Quantity > product.Quantity)
            {
                enoughItemsInStock = await CallToExternalStock(request, enoughItemsInStock, product);
            }

            Guard.Against.OutOfStock(enoughItemsInStock);

            isUpdated = _basketService.UpdateBasket(existingBasket, request, product);
            await _unitOfWork.SaveChangesAsync();

            return new BasketUpdateResponse(isUpdated);
        }

        private async Task<bool> CallToExternalStock(BasketUpdateRequest request, bool enoughItemsInStock, Product product)
        {
            var availableInExternalStock = _env.IsEnvironment("Test");
            _logger?.LogInformation("Call to external service started: ");
            _mockSupplierStockService
                .Setup(s => s.IsAvailableInStock(product.Id, request.Quantity))
                .ReturnsAsync(availableInExternalStock);

            enoughItemsInStock = await _mockSupplierStockService.Object.IsAvailableInStock(product.Id, request.Quantity);
            _logger?.LogInformation($"Response from external service:{enoughItemsInStock} ");

            return enoughItemsInStock;
        }
    }
}