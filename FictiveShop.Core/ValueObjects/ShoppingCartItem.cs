using Microsoft.EntityFrameworkCore;

namespace FictiveShop.Core.ValueObjects
{
    [Keyless]
    public class ShoppingCartItem : ValueObject
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