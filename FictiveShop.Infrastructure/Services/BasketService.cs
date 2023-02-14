using FictiveShop.Core.Domain;
using FictiveShop.Core.Dtos;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
using FictiveShop.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace FictiveShop.Infrastructure.Services
{
    public class BasketService : IBasketService
    {
        private readonly IInMemoryRedis _redisDb;
        private readonly IRepository<Product> _productsRepistory;
        private const int ThirtyDays = 30;

        public BasketService(IInMemoryRedis redisDb)
        {
            _redisDb = redisDb;
        }

        public bool UpdateBasket(CustomerBasket customerBasket, BasketUpdateDto request, Product product)
        {
            var updateProduct = customerBasket.Items.FirstOrDefault(x => x.ProductId == product.Id);
            if (updateProduct is not null)
            {
                updateProduct.Quantity += request.Quantity;

                return _redisDb.Set(request.CustomerId,
                                     JsonSerializer.Serialize(this),
                                     TimeSpan.FromDays(ThirtyDays));
            }

            customerBasket.Items.Add(new BasketItem
            {
                Price = product.Price.Amount,
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = product.Quantity
            });

            return _redisDb.Set(request.CustomerId,
                                    JsonSerializer.Serialize(this),
                                    TimeSpan.FromDays(ThirtyDays));
        }

        public bool AddToBasket(CustomerBasket customerBasket, BasketUpdateDto request, Product product)
        {
            bool isUpdated;

            customerBasket.Items = new List<BasketItem>
                        {
                            new BasketItem()
                            {
                                Price = product.Price.Amount,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                Quantity = request.Quantity
                            }
                        };

            isUpdated = _redisDb.Set(request.CustomerId,
                                     JsonSerializer.Serialize(this),
                                     TimeSpan.FromDays(ThirtyDays));
            return isUpdated;
        }
    }
}