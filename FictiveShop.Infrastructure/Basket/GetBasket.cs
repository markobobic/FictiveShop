using Ardalis.GuardClauses;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FictiveShop.Infrastructure.Basket
{
    public class GetBasket
    {
        public class Query : IRequest<BasketResponse>
        {
            public string CustomerId { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, BasketResponse>
        {
            private readonly IShoppingCartService _shoppingCartService;

            public QueryHandler(IShoppingCartService shoppingCartService)
            {
                _shoppingCartService = shoppingCartService;
            }

            public async Task<BasketResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var basketItems = _shoppingCartService.GetShoppingCartItems();
                Guard.Against.Null(basketItems, nameof(basketItems));
                return new BasketResponse
                {
                    Items = basketItems
                };
            }
        }

        public class BasketResponse
        {
            public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
        }
    }
}