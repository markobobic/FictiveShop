using FictiveShop.Core.Domain;
using FictiveShop.Core.ValueObjects;

namespace FictiveShop.Core.Interfeces
{
    public interface IShoppingCartService
    {
        void AddToCart(Product product);

        List<ShoppingCartItem> GetShoppingCartItems();
    }
}