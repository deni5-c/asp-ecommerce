using Microsoft.AspNetCore.Mvc;

namespace asp_ecommerce.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
