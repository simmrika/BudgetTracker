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
    public class CashTransactionModel : PageModel
    {
        private readonly ILogger<CashTransactionModel> _logger;
        private readonly CashTransactionDataService _transactionDataService;
        private readonly IHttpContextAccessor _contextAccessor;

        [BindProperty]
        public CashTransactionDto NewTransaction { get; set; } = new CashTransactionDto();

        public CashTransactionModel(ILogger<CashTransactionModel> logger, CashTransactionDataService transactionDataService, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _transactionDataService = transactionDataService;
            _contextAccessor = contextAccessor;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Re-display the form if the model state is invalid
            }

            NewTransaction.UserId = GetUserIdFromCookie();

            var result = await _transactionDataService.AddTransactionAsync(NewTransaction);
            if (result)
            {
                _logger.LogInformation("New transaction added successfully.");
                return RedirectToPage("/Transaction");
            }
            else
            {
                _logger.LogError("Error adding new transaction.");
                ModelState.AddModelError(string.Empty, "An error occurred while adding the transaction.");
                return Page(); // Re-display the form if there's an error
            }
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
