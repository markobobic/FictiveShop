using FictiveShop.Core.Interfeces;
using System.Threading.Tasks;

namespace FictiveShop.Infrastructure.Services
{
    public class SupplierStockService : ISupplierStockService
    {
        private readonly ISupplierStockService _supplierStockService;

        public SupplierStockService(ISupplierStockService supplierStockService)
        {
            _supplierStockService = supplierStockService;
        }

        public Task<bool> IsAvailableInStock(string prodcutId, int quantity)
        {
            return Task.FromResult(true);
        }
    }
}