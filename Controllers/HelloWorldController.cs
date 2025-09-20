using Microsoft.AspNetCore.Mvc;

namespace asp_ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHello()
        {
            return BadRequest(new
            {
                message = "Hello World!"
            });
        }
    }
}