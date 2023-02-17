using FictiveShop.Core.ValueObjects;

namespace FictiveShop.Core.Responses
{
    public record BasketGetByIdResponse(List<BasketItem> Items);
}