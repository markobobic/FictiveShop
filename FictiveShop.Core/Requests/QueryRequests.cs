using FictiveShop.Core.Interfeces;
using FictiveShop.Core.Responses;

namespace FictiveShop.Core.Requests
{
    public record BasketGetByCustomerIdRequest(string CustomerId) : IQuery<BasketGetByIdResponse>;
}