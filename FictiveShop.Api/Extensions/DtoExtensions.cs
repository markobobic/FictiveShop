using FictiveShop.Core.Domain;
using FictiveShop.Core.ValueObjects;
using static FictiveShop.Api.Features.Orders.CreateOrder;

namespace FictiveShop.Api.Extensions
{
    public static class DtoExtensions
    {
        public static Customer ToCustomer(this OrderRequest request) => new Customer
        {
            Id = request.CustomerId,
            PhoneNumber = request.PhoneNumber,
            Address = new Address
            {
                City = request.AddressRequest.City,
                HouseNumber = request.AddressRequest.HouseNumber,
                Street = request.AddressRequest.Street
            }
        };

        public static Order ToOrder(this Command request, CustomerBasket customerBasket, decimal discountPercentage)
        {
            var basketPrice = customerBasket.GetTotalPrice(discountPercentage);
            return new Order
            {
                TotalAmount = basketPrice.TotalPrice,
                CustomerId = request.Request.CustomerId,
                AppliedDiscount = basketPrice.DiscountedPrice,
                BasketItems = customerBasket.Items,
                ShippingAddress = new Address
                {
                    City = request.Request.AddressRequest.City,
                    HouseNumber = request.Request.AddressRequest.HouseNumber,
                    Street = request.Request.AddressRequest.Street
                }
            };
        }
    }
}