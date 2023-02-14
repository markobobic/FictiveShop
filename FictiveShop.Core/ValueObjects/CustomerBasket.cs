namespace FictiveShop.Core.ValueObjects
{
    public class CustomerBasket
    {
        public List<BasketItem> Items { get; set; } = new();
    }
}