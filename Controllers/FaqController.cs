using Microsoft.AspNetCore.Mvc;

namespace asp_ecommerce.Controllers
{
    public class FaqController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
