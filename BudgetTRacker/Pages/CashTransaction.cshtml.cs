using BudgetTracker.Service;
using BudgetTRacker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BudgetTRacker.Pages
{
    public class CashTransactionModel : PageModel
    {
        private readonly ILogger<CashTransactionModel> _logger;
        private readonly CashTransactionDataService _transactionDataService;

        [BindProperty]
        public CashTransactionDto NewTransaction { get; set; } = new CashTransactionDto();

        public CashTransactionModel(ILogger<CashTransactionModel> logger, CashTransactionDataService transactionDataService)
        {
            _logger = logger;
            _transactionDataService = transactionDataService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Re-display the form if the model state is invalid
            }
            
            var result = await _transactionDataService.AddTransactionAsync(NewTransaction);
            if (result)
            {
                _logger.LogInformation("New transaction added successfully.");
                return RedirectToPage("TransactionList"); // Redirect to a transaction list or another page after successful submission
            }
            else
            {
                _logger.LogError("Error adding new transaction.");
                ModelState.AddModelError(string.Empty, "An error occurred while adding the transaction.");
                return Page(); // Re-display the form if there's an error
            }
        }
    }
}
