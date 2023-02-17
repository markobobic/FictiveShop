using Ardalis.GuardClauses;

namespace FictiveShop.Core.Extensions
{
    public static class ListExtensions
    {
        public static void ReplaceOne<T>(this List<T> list, T newItem, T oldItem)
        {
            if (list is null) Guard.Against.Null(list, nameof(list), $"Db collection {typeof(T)} cannot be null.");
            int index = list.IndexOf(oldItem);

            if (index != -1)
            {
                list[index] = newItem;
            }
        }

        public static HashSet<TEntity> Set<TEntity>(this List<TEntity> list) => new HashSet<TEntity>(list);
    }
}