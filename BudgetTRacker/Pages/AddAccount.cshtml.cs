using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using BudgetTRacker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudgetTRacker.Pages
{
    [Authorize]
    public class AddAccountModel : PageModel
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public AddAccountModel(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _contextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public LinkedAccountRequest LinkedAccountRequest { get; set; }

        public void OnGet()
        {
        }

        // POST method to handle form submission
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Create a new LinkedAccount from the form data
                var linkedAccount = new LinkedAccounts
                {
                    UserID = GetUserIdFromCookie(), // Set the UserID based on the current logged-in user, e.g., get it from User.Identity
                    FirstName = LinkedAccountRequest.FirstName,
                    LastName = LinkedAccountRequest.LastName,
                    AccountNumber = LinkedAccountRequest.AccountNumber,
                    BankName = LinkedAccountRequest.BankName,
                    PhoneNumber = LinkedAccountRequest.PhoneNumber,
                    DateOfBirth = LinkedAccountRequest.DateOfBirth,
                    LinkedDate = DateTime.Now,
                    IsApproved = true // Initial status, you can change it later as needed
                };

                // Add the linked account to the database
                _appDbContext.LinkedAccount.Add(linkedAccount);
                await _appDbContext.SaveChangesAsync();

                // Redirect to a success or account listing page
                return RedirectToPage("/Account/Success");
            }

            // If the model state is invalid, stay on the same page with error messages
            return Page();
        }


        public int GetUserIdFromCookie()
        {
            var user = _contextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new Exception("User ID claim not found in the cookie.");
            }

            return int.Parse(userIdClaim.Value);
        }
    }
}
