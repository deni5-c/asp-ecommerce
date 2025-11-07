using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using asp_ecommerce.Services;

namespace asp_ecommerce.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly SupabaseAuthService _authService;

        public AdminController(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            ViewBag.UserName = User.Identity?.Name;
            ViewBag.UserEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            ViewBag.UserId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return View();
        }
    }
}