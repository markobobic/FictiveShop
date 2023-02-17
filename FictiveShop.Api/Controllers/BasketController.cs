using FictiveShop.Core.Requests;
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
            var basket = await _mediator.Send(new BasketGetByCustomerIdRequest(customerId));
            return Ok(basket.Items);
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket(BasketUpdateRequest request)
        {
            var basket = await _mediator.Send(request);
            if (basket.IsBasketUpdated is false) return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(basket);
        }
    }
}