using BudgetTracker.Service;
using BudgetTRacker.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BudgetTRacker.Pages
{
    [Authorize]
    public class CashEntryModel : PageModel
    {
        private readonly ILogger<CashEntryModel> _logger;
        private readonly AddCashDateService _addCashDateService;
        private readonly IHttpContextAccessor _contextAccessor;

        [BindProperty]
        public CashEntryDto NewCashEntry { get; set; } = new CashEntryDto();
        public string ErrorMessage { get; set; }

        public CashEntryModel(ILogger<CashEntryModel> logger, AddCashDateService addCashDateService, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _addCashDateService = addCashDateService;
            _contextAccessor = contextAccessor;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            NewCashEntry.AddDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields.";
                return Page(); // Re-display the form if the model state is invalid
            }

            try
            {
                // Assign user ID from the logged-in user
                NewCashEntry.UserId = GetUserIdFromCookie();

                

                // Add a new cash entry
                bool result = await _addCashDateService.AddCashEntryAsync(NewCashEntry);

                if (result)
                {
                    _logger.LogInformation("New cash entry added successfully.");
                    return RedirectToPage("/Index"); // Redirect after successful addition
                }
                else
                {
                    _logger.LogError("Error processing cash entry.");
                    ModelState.AddModelError(string.Empty, "An error occurred while processing the cash entry.");
                    return Page(); // Re-display the form if there's an error
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while adding the cash entry.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page(); // Re-display the form in case of an exception
            }
        }

        private int GetUserIdFromCookie()
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
