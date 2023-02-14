using FictiveShop.Api.Features.Basket;
using FictiveShop.Core.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FictiveShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        public readonly IMediator _mediator;

        public BasketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetBasketItems(string customerId)
        {
            var basket = await _mediator.Send(new GetBasket.Query { CustomerId = customerId });
            return Ok(basket.Items);
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket(BasketUpdateDto request)
        {
            var basket = await _mediator.Send(new AddOrUpdateBasket.Command { Request = request });
            if (basket.IsBasketUpdated is false) return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(basket);
        }
    }
}