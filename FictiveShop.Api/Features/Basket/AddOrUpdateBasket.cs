﻿using Ardalis.GuardClauses;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Dtos;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
using FictiveShop.Infrastructure.DataAccess;
using FluentValidation;
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

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Request.ProductId).NotNull().NotEmpty();
                RuleFor(x => x.Request.Quantity).GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<Command, BasketUpdateResponse>
        {
            private readonly FictiveShopDbContext _efDb;
            private readonly IInMemoryRedis _redisDb;
            private readonly IBasketService _basketService;
            private readonly ISupplierStockService _supplierStockService;
            private readonly IHostEnvironment _env;

            public Handler(FictiveShopDbContext efDb, IInMemoryRedis redisDb, IBasketService basketService, ISupplierStockService supplierStockService, IHostEnvironment env)
            {
                _efDb = efDb;
                _redisDb = redisDb;
                _basketService = basketService;
                _supplierStockService = supplierStockService;
                _env = env;
            }

            public async Task<BasketUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                bool enoughItemsInStock = true, isUpdated = false;
                var product = _efDb.Products.FirstOrDefault(x => x.Id == request.Request.ProductId);

                Guard.Against.Null(product, nameof(product), $"Product with ID: {request.Request.ProductId} doens't exist");
                if (request.Request.Quantity > product.Quantity)
                {
                    enoughItemsInStock = await CallToExternalStock(request, enoughItemsInStock, product);
                }

                Guard.Against.OutOfStock(enoughItemsInStock);

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

                Guard.Against.OutOfStock(enoughItemsInStock);

                isUpdated = _basketService.UpdateBasket(existingBasket, request.Request, product);
                return new BasketUpdateResponse { IsBasketUpdated = isUpdated };
            }

            private async Task<bool> CallToExternalStock(Command request, bool enoughItemsInStock, Product product)
            {
                if (_env.IsEnvironment("Test"))
                {
                    return await _supplierStockService.IsAvailableInStock(product.Id, request.Request.Quantity);
                }
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