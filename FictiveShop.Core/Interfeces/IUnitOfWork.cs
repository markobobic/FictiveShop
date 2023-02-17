namespace FictiveShop.Core.Interfeces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync();
    }
}