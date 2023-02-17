using Ardalis.GuardClauses;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.Requests;
using FictiveShop.Core.Responses;
using FictiveShop.Core.ValueObjects;
using FluentValidation;
using System.Text.Json;

namespace FictiveShop.Core.Features.Orders
{
    public class CreateOrderHandler : ICommandHandler<OrderRequest, OrderCreatedResponse>
    {
        private const int _4Pm = 16;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInMemoryRedis _redisDb;
        private readonly IRepository<Order> _ordersRepository;
        private readonly IRepository<Product> _productsRepository;
        private readonly IRepository<Customer> _customersRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CreateOrderHandler(
            IUnitOfWork unitOfWork,
            IInMemoryRedis redisDb,
            IRepository<Order> ordersRepository,
            IRepository<Product> productsRepository,
            IRepository<Customer> customerRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _redisDb = redisDb;
            _ordersRepository = ordersRepository;
            _productsRepository = productsRepository;
            _customersRepository = customerRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public class CommandValidator : AbstractValidator<OrderRequest>
        {
            public CommandValidator()
            {
                RuleFor(x => x.AddressRequest.City).NotNull().NotEmpty().WithMessage("City isn't populated.");
                RuleFor(x => x.AddressRequest.HouseNumber).NotNull().NotEmpty().WithMessage("HouseNumber isn't populated.");
                RuleFor(x => x.AddressRequest.Street).NotNull().NotEmpty().WithMessage("Street isn't populated.");
                RuleFor(x => x.PhoneNumber).NotNull().NotEmpty().WithMessage("Phone isn't populated."); ;
                RuleFor(x => x.CustomerId)
                    .NotEmpty()
                    .NotNull()
                    .Must(customerId => Guid.TryParse(customerId, out _))
                    .WithMessage("CustomerId must be a valid GUID.");
            }
        }

        public async Task<OrderCreatedResponse> Handle(OrderRequest request, CancellationToken cancellationToken)
        {
            var basketJson = _redisDb.Get(request.CustomerId);
            var customerBasket = basketJson.IsNullOrWhiteSpace()
                ? null
                : JsonSerializer.Deserialize<CustomerBasket>(basketJson);

            Guard.Against.Null(basketJson, nameof(basketJson), "Customer basket doesn't exists.");

            var discountPercentage = GetDiscountPrecentage(request);

            _customersRepository.Create(request.ToCustomer());

            var order = request.ToOrder(customerBasket, discountPercentage);
            _ordersRepository.Create(order);

            UpdateQuantity(customerBasket);

            ClearBasket(request);

            _unitOfWork.SaveChanges();

            return new(order.Id, order.TotalAmount, order.AppliedDiscount);
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

        private void ClearBasket(OrderRequest request) => _redisDb.Delete(request.CustomerId);

        private decimal GetDiscountPrecentage(OrderRequest request)
        {
            var hour = _dateTimeProvider.GetNow().Hour;
            if (hour == _4Pm)
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
}