using Ardalis.GuardClauses;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
using FictiveShop.Infrastructure.DataAccess;
using MediatR;
using Moq;
using static FictiveShop.Api.Features.Basket.AddOrUpdateBasket;

namespace FictiveShop.Api.Features.Order
{
    public class CreateOrder
    {
        public class Command : IRequest<OrderCreatedResponse>
        {
            public OrderRequest Request { get; set; }
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
            }
        }

        public class OrderCreatedResponse
        {
        }

        public class OrderRequest
        {
            public string CustomerId { get; set; }
            public AddressRequest AddressRequest { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class AddressRequest
        {
            public string City { get; set; }
            public string HouseNumber { get; set; }
            public string Street { get; set; }
        }
    }
}