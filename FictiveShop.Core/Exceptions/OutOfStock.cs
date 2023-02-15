namespace FictiveShop.Core.Exceptions
{
    public class OutOfStockException : Exception
    {
        public OutOfStockException(string message) : base(message)
        {
        }

        public OutOfStockException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}