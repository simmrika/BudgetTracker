using BudgetTRacker.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BudgetTRacker.Pages
{
    public class SignInModel : PageModel
    {
        private readonly IAuthService _authService;

        private readonly IHttpContextAccessor _contextAccessor;

        public SignInModel(IAuthService authService, IHttpContextAccessor contextAccessor)
        {
            _authService = authService;
            _contextAccessor = contextAccessor;
        }

        [BindProperty]
        public LoginDto LoginData { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }

            // Attempt to login using email or phone
            var user = await _authService.SignIn(LoginData.PhoneNumber, LoginData.Password);

            if (user == null)
            {
                ErrorMessage = "Invalid phone or password.";
                return Page();
            }
            // User authenticated, create claims for the user
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, $"{user.firstname} {user.firstname}"),
        new Claim(ClaimTypes.Email, user.email)
    };

            // Create a ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Create the cookie
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Keep the user logged in across sessions
                ExpiresUtc = DateTime.UtcNow.AddMinutes(10) // Set cookie expiration time
            };

            await _contextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/Index");
        }
    }

    public class LoginDto
    {
        public string PhoneNumber { get; set; } // Accepts phone number
        public string Password { get; set; }
    }
}
