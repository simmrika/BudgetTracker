using BudgetTRacker.Entities;
using BudgetTRacker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetTRacker.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly IAuthService _authService;

        public SignUpModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public SignupDto SignupData { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields.";
                return Page();
            }

            // Check if passwords match
            if (SignupData.Password != SignupData.ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            // Register the user
            var newUser = new User
            {
                firstname = SignupData.Firstname,
                lastname = SignupData.Lastname,
                email = SignupData.Email,
                phonenumber = SignupData.Phone,
                dateofbirth = SignupData.DateOfBirth

            };

            var registeredUser = await _authService.SignUp(newUser, SignupData.Password);
            if (registeredUser == null)
            {
                ErrorMessage = "Registration failed. Try again.";
                return Page();
            }

            SuccessMessage = "Registration successful! Please sign in.";
            return RedirectToPage("/SignIn");
        }
    }

    public class SignupDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}

