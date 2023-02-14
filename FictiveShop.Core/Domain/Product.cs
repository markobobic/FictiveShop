using FictiveShop.Core.ValueObjects;

namespace FictiveShop.Core.Domain
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public required Price Price { get; set; }
        public required int Quantity { get; set; }
    }
}