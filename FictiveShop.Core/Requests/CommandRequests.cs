using FictiveShop.Core.Interfeces;
using FictiveShop.Core.Responses;

namespace FictiveShop.Core.Requests
{
    public record OrderRequest(string CustomerId, AddressRequest AddressRequest, string PhoneNumber) : ICommand<OrderCreatedResponse>;
    public record AddressRequest(string City, string HouseNumber, string Street);
    public record BasketUpdateRequest(string CustomerId, string ProductId, int Quantity) : ICommand<BasketUpdateResponse>;
}