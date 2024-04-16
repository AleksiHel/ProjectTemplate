using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using System.Security.Claims;

namespace Project.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User model)
        {
            bool isAuthenticated = DatabaseManipulator.CheckPassword(model.Username.ToLower(), model.Password);

            if (isAuthenticated)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, model.Username.ToLower()) };

                if (model.Username == "admin")
                {
                    claims.Add(new Claim(ClaimTypes.Role, "admin"));
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                    IsPersistent = true
                };

                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            // Add error message to ModelState
            ModelState.AddModelError("LoginError", "Invalid username or password.");
            return View("Index", model);
        }

    }
}
