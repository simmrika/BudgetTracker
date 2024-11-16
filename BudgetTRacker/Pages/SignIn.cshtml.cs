using BudgetTRacker.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetTRacker.Pages
{
    public class SignInModel : PageModel
    {
        private readonly IAuthService _authService;

        public SignInModel(IAuthService authService)
        {
            _authService = authService;
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
            var token = await _authService.SignIn(LoginData.PhoneNumber, LoginData.Password);

            if (token == null)
            {
                ErrorMessage = "Invalid phone or password.";
                return Page();
            }

            // Store token as needed (e.g., in a cookie, local storage, etc.)
            HttpContext.Response.Cookies.Append("AuthToken", token);

            return RedirectToPage("/Index");
        }
    }

    public class LoginDto
    {
        public string PhoneNumber { get; set; } // Accepts phone number
        public string Password { get; set; }
    }
}
