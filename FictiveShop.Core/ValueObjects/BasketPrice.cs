namespace FictiveShop.Core.ValueObjects
{
    public class BasketPrice : ValueObject
    {
        private decimal _totalPrice;
        public decimal TotalPrice { get => _totalPrice; init => _totalPrice = value; }
        public decimal DiscountedPrice { get; set; }

        public void SetTotalPrice(decimal value) => _totalPrice = value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TotalPrice;
            yield return DiscountedPrice;
        }
    }
}