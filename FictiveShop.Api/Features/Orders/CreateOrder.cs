using Ardalis.GuardClauses;
using FictiveShop.Api.Extensions;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
using FluentValidation;
using MediatR;
using System.Text.Json;

namespace FictiveShop.Api.Features.Orders
{
    public class CreateOrder
    {
        public class Command : IRequest<OrderCreatedResponse>
        {
            public OrderRequest Request { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Request.AddressRequest.City).NotNull().NotEmpty().WithMessage("City isn't populated.");
                RuleFor(x => x.Request.AddressRequest.HouseNumber).NotNull().NotEmpty().WithMessage("HouseNumber isn't populated.");
                RuleFor(x => x.Request.AddressRequest.Street).NotNull().NotEmpty().WithMessage("Street isn't populated.");
                RuleFor(x => x.Request.PhoneNumber).NotNull().NotEmpty().WithMessage("Phone isn't populated."); ;
                RuleFor(x => x.Request.CustomerId)
                    .NotEmpty()
                    .NotNull()
                    .Must(customerId => Guid.TryParse(customerId, out _))
                    .WithMessage("CustomerId must be a valid GUID.");
            }
        }

        public class Handler : IRequestHandler<Command, OrderCreatedResponse>
        {
            private const int _4Pm = 16;
            private const int _5Pm = 17;
            private readonly IInMemoryRedis _redisDb;
            private readonly IRepository<Order> _ordersRepository;
            private readonly IRepository<Product> _productsRepository;
            private readonly IRepository<Customer> _customersRepository;

            public Handler(IInMemoryRedis redisDb, IRepository<Order> ordersRepository, IRepository<Product> productsRepository, IRepository<Customer> customerRepository)
            {
                _redisDb = redisDb;
                _ordersRepository = ordersRepository;
                _productsRepository = productsRepository;
                _customersRepository = customerRepository;
            }

            public async Task<OrderCreatedResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var basketJson = _redisDb.Get(request.Request.CustomerId);
                var customerBasket = basketJson.IsNullOrWhiteSpace()
                    ? null :
                    JsonSerializer.Deserialize<CustomerBasket>(basketJson);

                Guard.Against.Null(basketJson, nameof(basketJson), "Customer basket doesn't exists.");

                var discountPercentage = GetDiscountPrecentage(customerBasket, request.Request);

                _customersRepository.Create(request.Request.ToCustomer());

                var order = request.ToOrder(customerBasket, discountPercentage);
                _ordersRepository.Create(order);

                UpdateQuantity(customerBasket);

                ClearBasket(request);

                return new()
                {
                    AppliedDiscount = order.AppliedDiscount,
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount
                };
            }

            private void UpdateQuantity(CustomerBasket customerBasket)
            {
                foreach (var basketItem in customerBasket.Items)
                {
                    var product = _productsRepository.GetById(basketItem.ProductId);

                    if (product is null)
                        Guard.Against.Null(product, nameof(product), "Product in orderded items is null.");

                    product.Quantity -= basketItem.Quantity;
                    _productsRepository.Update(product);
                }
            }

            private void ClearBasket(Command request) => _redisDb.Delete(request.Request.CustomerId);

            private decimal GetDiscountPrecentage(CustomerBasket customerBasket, OrderRequest request)
            {
                var hour = DateTime.Now.Hour;
                if (hour == _4Pm || hour == _5Pm)
                {
                    var lastDigit = request.PhoneNumber.ExtractNumber().Last().ToString().ToInt();
                    return lastDigit switch
                    {
                        0 => 0.3m,
                        var digit when digit % 2 == 0 => 0.2m,
                        var digit when digit % 2 == 1 => 0.1m,
                        _ => 0m
                    };
                }
                return 0;
            }
        }

        public class OrderCreatedResponse
        {
            public string OrderId { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal AppliedDiscount { get; set; }
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