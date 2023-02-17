namespace FictiveShop.Core.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue);
    }
}