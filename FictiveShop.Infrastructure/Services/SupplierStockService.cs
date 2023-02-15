using FictiveShop.Core.Interfeces;
using System.Threading.Tasks;

namespace FictiveShop.Infrastructure.Services
{
    public class SupplierStockService : ISupplierStockService
    {
        public Task<bool> IsAvailableInStock(string prodcutId, int quantity)
        {
            return Task.FromResult(true);
        }
    }
}