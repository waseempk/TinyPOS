using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TinyPOSApp.Data;
using TinyPOSApp.Models;

namespace TinyPOSApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly TinyPOSContext _context;
        private readonly IWebHostEnvironment _env;

        public AccountController(TinyPOSContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.Profiles
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Pin == model.Pin);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FirstName ?? user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role ?? "CASHIER")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId)) return RedirectToAction("Login");

            var profile = await _context.Profiles.FindAsync(userId);
            if (profile == null) return NotFound();

            var model = new ProfileUpdateViewModel
            {
                FirstName = profile.FirstName ?? string.Empty,
                LastName = profile.LastName ?? string.Empty,
                Email = profile.Email,
                ImageUrl = profile.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Profile(ProfileUpdateViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId)) return RedirectToAction("Login");

            var profile = await _context.Profiles.FindAsync(userId);
            if (profile == null) return NotFound();

            // Note: Email is read-only according to user request, so we don't update it from model.Email.
            model.Email = profile.Email; // Reinforce UI

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verify PIN change if requested
            bool pinChanged = false;
            if (!string.IsNullOrEmpty(model.NewPin))
            {
                if (model.CurrentPin != profile.Pin)
                {
                    ModelState.AddModelError("CurrentPin", "Current PIN is incorrect.");
                    return View(model);
                }
                
                profile.Pin = model.NewPin;
                pinChanged = true;
            }

            profile.FirstName = model.FirstName;
            profile.LastName = model.LastName;

            if (model.RemoveImage)
            {
                profile.ImageUrl = null;
            }

            if (model.ProfileImage != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }
                profile.ImageUrl = "/uploads/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();

            if (pinChanged)
            {
                // Force logout upon PIN change based on user request
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }
    }
}
