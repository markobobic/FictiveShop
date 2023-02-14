namespace FictiveShop.Core.Interfeces
{
    public interface ISupplierStockService
    {
        Task<bool> IsAvailableInStock(string prodcutId, int quantity);
    }
}