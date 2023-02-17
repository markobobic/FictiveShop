using Ardalis.GuardClauses;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.Requests;
using FictiveShop.Core.Responses;
using FictiveShop.Core.ValueObjects;
using FluentValidation;
using System.Text.Json;

namespace FictiveShop.Core.Features.Basket
{
    public class GetBasketHandler : IQueryHandler<BasketGetByCustomerIdRequest, BasketGetByIdResponse>
    {
        private readonly IInMemoryRedis _redis;

        public GetBasketHandler(IInMemoryRedis redis)
        {
            _redis = redis;
        }

        public class CommandValidator : AbstractValidator<BasketGetByCustomerIdRequest>
        {
            public CommandValidator()
            {
                RuleFor(x => x.CustomerId).NotNull().NotEmpty();
            }
        }

        public async Task<BasketGetByIdResponse> Handle(BasketGetByCustomerIdRequest request, CancellationToken cancellationToken)
        {
            var basketJson = _redis.Get(request.CustomerId);
            var customerBasket = basketJson.IsNullOrWhiteSpace() ? null : JsonSerializer.Deserialize<CustomerBasket>(basketJson);
            Guard.Against.Null(customerBasket, nameof(customerBasket), $"No basket found for customer with ID {request.CustomerId}");
            return new BasketGetByIdResponse(customerBasket.Items);
        }
    }
}