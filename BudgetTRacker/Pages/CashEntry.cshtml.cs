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

                // Check if the user already has a cash entry
                var existingCashEntry = await _addCashDateService.GetCashEntryByUserIdAsync(NewCashEntry.UserId);

                bool result;

                if (existingCashEntry == null)
                {
                    // If no existing entry, add a new cash entry
                    result = await _addCashDateService.AddCashEntryAsync(NewCashEntry);
                }
                else
                {
                    // If an entry exists, update it
                    existingCashEntry.Amount += NewCashEntry.Amount; // You can modify this logic to overwrite the amount instead
                    existingCashEntry.AddDate = NewCashEntry.AddDate;
                    result = await _addCashDateService.UpdateCashEntryAsync(existingCashEntry);
                }

                if (result)
                {
                    _logger.LogInformation(existingCashEntry == null ? "New cash entry added successfully." : "Cash entry updated successfully.");
                    return RedirectToPage("/Index"); // Redirect after successful addition or update
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

        private void ValidateCashEntry(CashEntryDto cashEntry)
        {
            if (cashEntry == null)
            {
                throw new ArgumentNullException(nameof(cashEntry), "Cash entry cannot be null.");
            }

            if (cashEntry.Amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.");
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
