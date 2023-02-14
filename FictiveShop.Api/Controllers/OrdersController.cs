using FictiveShop.Api.Features.Orders;
using MediatR;
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
        public async Task<IActionResult> PostOrder(CreateOrder.OrderRequest request)
        {
            var basket = await _mediator.Send(new CreateOrder.Command { Request = request });

            return Ok(basket);
        }
    }
}