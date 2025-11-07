using Microsoft.AspNetCore.Mvc;

namespace asp_ecommerce.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
