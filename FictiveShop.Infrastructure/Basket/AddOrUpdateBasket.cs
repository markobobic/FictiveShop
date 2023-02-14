using Ardalis.GuardClauses;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Infrastructure.DataAccess;
using MediatR;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FictiveShop.Infrastructure.Basket
{
    public class AddToBasket
    {
        public class Command : IRequest<BasketUpdateResponse>
        {
            public BasketUpdateRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, BasketUpdateResponse>
        {
            private readonly FictiveShopDbContext _efDb;
            private readonly IShoppingCartService _shoppingCartService;

            public Handler(FictiveShopDbContext efDb, IShoppingCartService shoppingCartService)
            {
                _efDb = efDb;
                _shoppingCartService = shoppingCartService;
            }

            public async Task<BasketUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                bool enoughItemsInStock = true;
                var product = _efDb.Products.FirstOrDefault(x => x.Id == request.Request.ProductId);

                Guard.Against.Null(product, nameof(product));
                if (request.Request.Quantity > product.Quantity)
                {
                    var mockSupplierStockService = new Mock<ISupplierStockService>();
                    mockSupplierStockService
                        .Setup(s => s.IsAvailableInStock(product.Id, request.Request.Quantity))
                        .ReturnsAsync(false);
                    enoughItemsInStock = await mockSupplierStockService.Object.IsAvailableInStock(product.Id, product.Quantity);
                }

                Guard.Against.False(enoughItemsInStock, "Not enough items in stock.");

                _shoppingCartService.AddToCart(product);

                return new BasketUpdateResponse { IsBasketUpdated = enoughItemsInStock };
            }
        }

        public class BasketUpdateRequest
        {
            public string CustomerId { get; set; }
            public string ProductId { get; set; }
            public int Quantity { get; set; }
        }

        public class BasketUpdateResponse
        {
            public bool IsBasketUpdated { get; set; }
        }
    }
}