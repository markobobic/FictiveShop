using FictiveShop.Core.Interfeces;

namespace FictiveShop.Core.Settings
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetNow() => DateTime.Now;
    }
}