using BudgetTRacker.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetTRacker.Pages
{ 

    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly BankTransactionDataService _bankTransactionDataService;

        [BindProperty]
        public IEnumerable<TransactionDto> Transactions { get; private set; } = new List<TransactionDto>();

        public IndexModel(ILogger<IndexModel> logger, BankTransactionDataService bankTransactionDataService)
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
