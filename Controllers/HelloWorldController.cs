using Microsoft.AspNetCore.Mvc;

namespace asp_ecommerce.Controllers
{
    public class HelloWorldController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "Hello World";

            return View();
        }
    }
}
