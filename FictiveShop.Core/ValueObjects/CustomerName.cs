using Microsoft.EntityFrameworkCore;

namespace FictiveShop.Core.ValueObjects
{
    [Keyless]
    public class CustomerName : ValueObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string GetFullName() => $"{FirstName} {LastName}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
    }
}