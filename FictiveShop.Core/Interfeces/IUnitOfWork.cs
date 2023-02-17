using FictiveShop.Core.Domain;

namespace FictiveShop.Core.Interfeces
{
    public interface IUnitOfWork
    {
        bool SaveChanges();
    }
}