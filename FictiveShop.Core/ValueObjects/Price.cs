namespace FictiveShop.Core.ValueObjects
{
    public class Price : ValueObject
    {
        public Price(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency?.ToUpper();
        }

        public Price()
        {
        }

        public virtual decimal Amount { get; init; }

        public string Currency { get; init; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}