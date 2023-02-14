namespace FictiveShop.Core.ValueObjects
{
    public class CustomerBasket
    {
        public List<BasketItem> Items { get; set; } = new();
        public decimal DiscountPrecentege { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TotalPrice => GetPrice();

        private decimal GetPrice()
        {
            var totalPrice = Items.Sum(x => x.Quantity * x.Price);
            if (DiscountPrecentege != 0)
            {
                DiscountAmount = totalPrice * DiscountPrecentege;
                totalPrice = totalPrice * (1 - DiscountPrecentege);
            }
            return totalPrice;
        }
    }
}