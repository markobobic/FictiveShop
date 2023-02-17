using FictiveShop.Core.Domain;
using FictiveShop.Core.Requests;
using FictiveShop.Core.ValueObjects;

namespace FictiveShop.Core.Interfeces
{
    public interface IBasketService
    {
        bool AddToBasket(CustomerBasket customerBasket, BasketUpdateRequest request, Product product);

        bool UpdateBasket(CustomerBasket customerBasket, BasketUpdateRequest request, Product product);
    }
}