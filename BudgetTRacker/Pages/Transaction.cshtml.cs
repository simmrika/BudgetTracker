using BudgetTRacker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetTRacker.Pages
{
    public class TransactionModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BankTransactionDataService _bankTransactionDataService;

        [BindProperty]
        public IEnumerable<TransactionDto> Transactions { get; private set; } = new List<TransactionDto>();

        public TransactionModel(ILogger<IndexModel> logger, BankTransactionDataService bankTransactionDataService)
        {
            _logger = logger;
            _bankTransactionDataService = bankTransactionDataService;
        }

        public async Task OnGetAsync()
        {
            Transactions = await _bankTransactionDataService.GetTransactionsByUserIdAsync(1);
        }
    }
}
