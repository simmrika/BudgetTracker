using BudgetTRacker.Entities;
using BudgetTRacker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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

        public string DOBError { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields.";
                return Page();
            }

            // Custom validation for Date of Birth
            if (SignupData.DateOfBirth > DateTime.Now)
            {
                DOBError = "Date of Birth cannot be in the future.";
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

            TempData["SuccessMessage"] = "Registration successful! Please sign in.";

            return RedirectToPage("/SignIn");
        }
    }

    public class SignupDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

    }
}

