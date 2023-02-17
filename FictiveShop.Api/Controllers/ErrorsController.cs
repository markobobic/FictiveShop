using FictiveShop.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FictiveShop.Api.Controllers
{
    [Route("errors/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ErrorsController : ControllerBase
    {
        public IActionResult Error(int code) => new ObjectResult(new ApiResponse(code));
    }
}