using FictiveShop.Core.Interfeces;
using System.Threading.Tasks;

namespace FictiveShop.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FictiveShopDbContext _context;

        public UnitOfWork(FictiveShopDbContext _context)
        {
            this._context = _context;
        }

        public async Task<bool> SaveChangesAsync() => await _context.SaveChanges();
    }
}