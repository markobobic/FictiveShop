using Ardalis.GuardClauses;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
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

        public class Handler : IRequestHandler<Command, OrderCreatedResponse>
        {
            private const int _4Pm = 16;
            private const int _5Pm = 17;
            private readonly IInMemoryRedis _redisDb;
            private readonly IRepository<Order> _orderRepository;
            private readonly IRepository<Product> _productsRepository;

            public Handler(IInMemoryRedis redisDb, IRepository<Order> ordersRepository, IRepository<Product> productsRepository)
            {
                _redisDb = redisDb;
                _orderRepository = ordersRepository;
                _productsRepository = productsRepository;
            }

            public async Task<OrderCreatedResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var basketJson = _redisDb.Get(request.Request.CustomerId);
                var customerBasket = basketJson.IsNullOrWhiteSpace() ? null : JsonSerializer.Deserialize<CustomerBasket>(basketJson);

                Guard.Against.Null(basketJson, nameof(basketJson), "Customer basket doesn't exists.");
                var order = new Order();
                ApplyDiscount(customerBasket, request.Request, order);
                _orderRepository.Create(order);

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

            private void ClearBasket(Command request)
            {
                _redisDb.Delete(request.Request.CustomerId);
            }

            private void ApplyDiscount(CustomerBasket customerBasket, OrderRequest request, Order order)
            {
                var hour = DateTime.Now.Hour;

                if (hour == _4Pm || hour == _5Pm)
                {
                    var lastDigit = request.PhoneNumber.ExtractNumber().Last().ToString().ToInt();
                    customerBasket.DiscountPrecentege = lastDigit switch
                    {
                        0 => 0.3m,
                        var digit when digit % 2 == 0 => 0.2m,
                        var digit when digit % 2 == 1 => 0.1m,
                        _ => 0m
                    };
                }
                order.TotalAmount = customerBasket.TotalPrice;
                order.AppliedDiscount = customerBasket.DiscountAmount;
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