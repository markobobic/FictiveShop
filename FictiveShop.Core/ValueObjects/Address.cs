using Microsoft.EntityFrameworkCore;

namespace FictiveShop.Core.ValueObjects
{
    [Keyless]
    public class Address : ValueObject
    {
        public string City { get; set; }
        public string HouseNumber { get; set; }
        public string Street { get; set; }

        public string GetFullAddress() => $"{Street} {HouseNumber}, {City}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return City;
            yield return HouseNumber;
            yield return Street;
        }
    }
}