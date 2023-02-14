using FictiveShop.Core.Domain;
using FictiveShop.Core.Dtos;
using FictiveShop.Core.ValueObjects;

namespace FictiveShop.Core.Interfeces
{
    public interface IBasketService
    {
        bool AddToBasket(CustomerBasket customerBasket, BasketUpdateDto request, Product product);

        bool UpdateBasket(CustomerBasket customerBasket, BasketUpdateDto request, Product product);
    }
}