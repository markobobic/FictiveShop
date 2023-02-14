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
    }
}