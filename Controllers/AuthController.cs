using asp_ecommerce.Models;
using asp_ecommerce.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace asp_ecommerce.Controllers
{
    public class AuthController : Controller
    {
        private readonly SupabaseAuthService _authService;

        public AuthController(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.RegisterAsync(model.Name, model.Email, model.Password);

            if (result.Success && result.User != null)
            {
                // Automatically log in the user after registration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.User.Id),
                    new Claim(ClaimTypes.Name, result.User.Name),
                    new Claim(ClaimTypes.Email, result.User.Email),
                    new Claim(ClaimTypes.Role, result.User.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["SuccessMessage"] = "Welcome! Your account has been created successfully.";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model.Email, model.Password);

            if (result.Success && result.User != null)
            {
                // Create claims for the user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.User.Id),
                    new Claim(ClaimTypes.Name, result.User.Name),
                    new Claim(ClaimTypes.Email, result.User.Email),
                    new Claim(ClaimTypes.Role, result.User.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["SuccessMessage"] = "Welcome back, " + result.User.Name + "!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // POST: /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] = "You have been logged out successfully";
            return RedirectToAction("Login");
        }

        // GET: /Auth/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Auth/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.SendPasswordResetEmailAsync(model.Email);

            if (result.Success)
            {
                TempData["ResetEmail"] = model.Email;
                TempData["SuccessMessage"] = "Please proceed to reset your password.";
                return RedirectToAction("ResetPassword");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // GET: /Auth/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword()
        {
            var email = TempData["ResetEmail"] as string;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            TempData.Keep("ResetEmail");

            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        // POST: /Auth/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email ?? TempData["ResetEmail"] as string;

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Invalid reset request. Please try again.");
                return RedirectToAction("ForgotPassword");
            }

            var result = await _authService.ResetPasswordAsync(email, model.Password);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // GET: /Auth/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}