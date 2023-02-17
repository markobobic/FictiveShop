namespace FictiveShop.Core.Responses
{
    public record OrderCreatedResponse(string OrderId, decimal TotalAmount, decimal AppliedDiscount);
    public record BasketUpdateResponse(bool IsBasketUpdated);
}