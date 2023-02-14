using FictiveShop.Core.Domain;
using System.Linq.Expressions;

namespace FictiveShop.Core.Interfeces
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Create(T entity);

        IReadOnlyCollection<T> GetAll();

        IReadOnlyCollection<T> GetAll(Func<T, bool> filter);

        T GetById(string id);

        void Remove(string id);

        void Update(T entity);
    }
}