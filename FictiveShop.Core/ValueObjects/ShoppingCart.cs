using FictiveShop.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace FictiveShop.Core.ValueObjects
{
    [Keyless]
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; } = new();

        public void AddItem(Product product)
        {
            var shoppingCartItem = new ShoppingCartItem
            {
                Price = product.Price.Amount,
                ProductName = product.Name,
                Quantity = product.Quantity,
                ProductId = product.Id
            };

            Items.Add(shoppingCartItem);
        }

        public List<ShoppingCartItem> GetItems()
        {
            return Items;
        }
    }
}