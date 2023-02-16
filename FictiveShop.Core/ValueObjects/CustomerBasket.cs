namespace FictiveShop.Core.ValueObjects
{
    public class CustomerBasket
    {
        public List<BasketItem> Items { get; set; }

        public BasketPrice GetTotalPrice(decimal discountPrecentage = 0)
        {
            var basketPrice = new BasketPrice
            {
                TotalPrice = Items.Sum(x => x.Quantity * x.Price)
            };

            if (discountPrecentage != 0)
            {
                basketPrice.DiscountedPrice = basketPrice.TotalPrice * discountPrecentage;
                basketPrice.SetTotalPrice(basketPrice.TotalPrice * (1 - discountPrecentage));
            }
            return basketPrice;
        }
    }
}