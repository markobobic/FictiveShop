namespace FictiveShop.Core.Domain
{
    public class Order : BaseEntity
    {
        public decimal TotalAmount { get; set; }
        public decimal AppliedDiscount { get; set; }
    }
}