using Ardalis.GuardClauses;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Dtos;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
using FictiveShop.Infrastructure.DataAccess;
using MediatR;
using Moq;
using System.Text.Json;

namespace FictiveShop.Api.Features.Basket
{
    public class AddOrUpdateBasket
    {
        public class Command : IRequest<BasketUpdateResponse>
        {
            public BasketUpdateDto Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, BasketUpdateResponse>
        {
            private readonly FictiveShopDbContext _efDb;
            private readonly IInMemoryRedis _redisDb;
            private readonly IBasketService _basketService;

            public Handler(FictiveShopDbContext efDb, IInMemoryRedis redisDb, IBasketService basketService)
            {
                _efDb = efDb;
                _redisDb = redisDb;
                _basketService = basketService;
            }

            public async Task<BasketUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                bool enoughItemsInStock = true, isUpdated = false;
                var product = _efDb.Products.FirstOrDefault(x => x.Id == request.Request.ProductId);

                Guard.Against.Null(product, nameof(product));
                if (request.Request.Quantity > product.Quantity)
                {
                    enoughItemsInStock = await CallToExternalStock(request, enoughItemsInStock, product);
                }

                Guard.Against.False(enoughItemsInStock, "Not enough items in stock.");

                if (_redisDb.Get(request.Request.CustomerId) is null)
                {
                    var newBasket = new CustomerBasket();
                    isUpdated = _basketService.AddToBasket(newBasket, request.Request, product);

                    return new BasketUpdateResponse { IsBasketUpdated = isUpdated };
                }

                var basketData = _redisDb.Get(request.Request.CustomerId);
                var existingBasket = JsonSerializer.Deserialize<CustomerBasket>(basketData);
                var existingProduct = existingBasket.Items.FirstOrDefault(x => x.ProductId == request.Request.ProductId);
                
                var existingProductQuantity = existingProduct?.Quantity ?? 0;
                if (existingProductQuantity + request.Request.Quantity > product.Quantity)
                {
                    enoughItemsInStock = await CallToExternalStock(request, enoughItemsInStock, product);
                }

                Guard.Against.False(enoughItemsInStock, "Not enough items in stock.");

                isUpdated = _basketService.UpdateBasket(existingBasket, request.Request, product);
                return new BasketUpdateResponse { IsBasketUpdated = isUpdated };
            }

            private async Task<bool> CallToExternalStock(Command request, bool enoughItemsInStock, Product product)
            {
                var mockSupplierStockService = new Mock<ISupplierStockService>();
                mockSupplierStockService
                    .Setup(s => s.IsAvailableInStock(product.Id, request.Request.Quantity))
                    .ReturnsAsync(false);
                enoughItemsInStock = await mockSupplierStockService.Object.IsAvailableInStock(product.Id, product.Quantity);
                return enoughItemsInStock;
            }
        }

        public class BasketUpdateResponse
        {
            public bool IsBasketUpdated { get; set; }
        }
    }
}