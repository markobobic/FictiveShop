using Ardalis.GuardClauses;
using FictiveShop.Core.Exceptions;

namespace FictiveShop.Core.Extensions
{
    public static class GuardExtensions
    {
        public static void OutOfStock(this IGuardClause guardClause, bool condition, string errMsg = "")
        {
            if (!condition)
            {
                throw new OutOfStockException(errMsg == string.Empty ? "Not enough items in stock." : errMsg);
            }
        }
    }
}