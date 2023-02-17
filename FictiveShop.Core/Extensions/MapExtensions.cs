using FictiveShop.Core.Domain;
using FictiveShop.Core.Requests;
using FictiveShop.Core.ValueObjects;

namespace FictiveShop.Core.Extensions
{
    public static class MapExtensions
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

        public static Order ToOrder(this OrderRequest request, CustomerBasket customerBasket, decimal discountPercentage)
        {
            var basketPrice = customerBasket.GetTotalPrice(discountPercentage);
            return new Order
            {
                TotalAmount = basketPrice.TotalPrice,
                CustomerId = request.CustomerId,
                AppliedDiscount = basketPrice.DiscountedPrice,
                BasketItems = customerBasket.Items,
                ShippingAddress = new Address
                {
                    City = request.AddressRequest.City,
                    HouseNumber = request.AddressRequest.HouseNumber,
                    Street = request.AddressRequest.Street
                }
            };
        }
    }
}