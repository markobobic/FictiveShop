using FictiveShop.Api.Features.Basket;
using FictiveShop.Core.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FictiveShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> PostOrder(BasketUpdateDto request)
        {
            var basket = await _mediator.Send(new AddOrUpdateBasket.Command { Request = request });
            if (basket.IsBasketUpdated is false) return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(basket);
        }
    }
}