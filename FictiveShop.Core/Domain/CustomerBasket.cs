namespace FictiveShop.Core.Domain
{
    public class CustomerBasket : BaseEntity
    {
        public List<Product> Items { get; set; } = new List<Product>();
        private long _totalPrice;

        public long TotalPrice
        {
            get { return _totalPrice = (long)Items.Sum(x => x.Quantity * (x.Price.Amount * 100)); }
            private set { _totalPrice = (long)Items.Sum(x => x.Quantity * (x.Price.Amount * 100)); }
        }
    }
}