namespace FictiveShop.Core.Extensions
{
    public static class ListExtensions
    {
        public static void ReplaceOne<T>(this List<T> list, T newItem, T oldItem)
        {
            if (list is null) return;
            int index = list.IndexOf(oldItem);

            if (index != -1)
            {
                list[index] = newItem;
            }
        }

        public static HashSet<TEntity> Set<TEntity>(this List<TEntity> list)
        {
            return new HashSet<TEntity>(list);
        }

        public static void AddEntity<TEntity>(this List<TEntity> list, TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            list.Add(entity);
        }
    }
}