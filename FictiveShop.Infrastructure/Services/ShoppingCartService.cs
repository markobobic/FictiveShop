using FictiveShop.Core.Domain;
using FictiveShop.Core.Interfeces;
using FictiveShop.Core.ValueObjects;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FictiveShop.Infrastructure.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddToCart(Product product)
        {
            var shoppingCartCookie = _httpContextAccessor.HttpContext.Request.Cookies["ShoppingCart"];
            var shoppingCart = new ShoppingCart();
            if (!string.IsNullOrEmpty(shoppingCartCookie))
            {
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartCookie);
            }

            shoppingCart.AddItem(product);
            var shoppingCartJson = JsonConvert.SerializeObject(shoppingCart.Items);
            _httpContextAccessor.HttpContext.Response.Cookies.Append("ShoppingCart", shoppingCartJson, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
            });
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            var shoppingCartCookie = _httpContextAccessor.HttpContext.Request.Cookies["ShoppingCart"];
            if (string.IsNullOrEmpty(shoppingCartCookie))
            {
                return new List<ShoppingCartItem>();
            }

            var shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartCookie);
            return shoppingCart.GetItems();
        }
    }
}