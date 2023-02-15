using Ardalis.GuardClauses;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
using FluentValidation;
using MediatR;
using System.Text.Json;

namespace FictiveShop.Api.Features.Basket
{
    public class GetBasket
    {
        public class Query : IRequest<BasketResponse>
        {
            public string CustomerId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Query>
        {
            public CommandValidator()
            {
                RuleFor(x => x.CustomerId).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, BasketResponse>
        {
            private readonly IInMemoryRedis _redis;

            public QueryHandler(IInMemoryRedis redis)
            {
                _redis = redis;
            }

            public async Task<BasketResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var basketJson = _redis.Get(request.CustomerId);
                var customerBasket = basketJson.IsNullOrWhiteSpace() ? null : JsonSerializer.Deserialize<CustomerBasket>(basketJson);
                Guard.Against.Null(customerBasket, nameof(customerBasket), $"No basket found for customer with ID {request.CustomerId}");
                return new BasketResponse { Items = customerBasket.Items };
            }
        }

        public class BasketResponse
        {
            public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        }
    }
}