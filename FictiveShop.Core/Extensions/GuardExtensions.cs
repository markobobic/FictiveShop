using Ardalis.GuardClauses;
using FictiveShop.Core.Exceptions;

namespace FictiveShop.Core.Extensions
{
    public static class GuardExtensions
    {
        public static void True(this IGuardClause guardClause, bool condition, string errMsg = "")
        {
            if (condition is true)
                throw new Exception(errMsg);
        }

        public static void False(this IGuardClause guardClause, bool condition, string errMsg = "")
        {
            if (condition is false)
                throw new Exception(errMsg);
        }
    }
}