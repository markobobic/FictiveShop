namespace FictiveShop.Core.ValueObjects
{
    public class BasketItem : ValueObject
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ProductId { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductName;
            yield return Price;
            yield return Quantity;
            yield return ProductId;
        }
    }
}