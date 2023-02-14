namespace FictiveShop.Core.Dtos
{
    public class BasketUpdateDto
    {
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}