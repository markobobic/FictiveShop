using FictiveShop.Core.Requests;
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
        public async Task<IActionResult> PostOrder(OrderRequest request)
        {
            var order = await _mediator.Send(request);

            return Ok(order);
        }
    }
}