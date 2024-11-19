using BudgetTracker.Service;
using BudgetTRacker.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetTRacker.Pages
{
    [Authorize]
    public class TransactionModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BankTransactionDataService _bankTransactionDataService;
        private readonly CashTransactionDataService _cashTransactionDataService;
        private readonly IHttpContextAccessor _contextAccessor;


        [BindProperty]
        public IEnumerable<TransactionDto> Transactions { get; private set; } = new List<TransactionDto>();

        [BindProperty]
        public IEnumerable<CashTransactionDto> CashTransactions { get; private set; } = new List<CashTransactionDto>();

        public TransactionModel(ILogger<IndexModel> logger, BankTransactionDataService bankTransactionDataService, CashTransactionDataService cashTransactionDataService, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _bankTransactionDataService = bankTransactionDataService;
            _cashTransactionDataService = cashTransactionDataService;
            _contextAccessor = contextAccessor;
        }

        public async Task OnGetAsync()
        {
            var userId = GetUserIdFromCookie();
            
            Transactions = await _bankTransactionDataService.GetTransactionsByUserIdAsync(userId);
            CashTransactions = await _cashTransactionDataService.GetTransactionsByUserIdAsync(userId);
        }

        public int GetUserIdFromCookie()
        {
            var user = _contextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new Exception("User ID claim not found in the cookie.");
            }

            return int.Parse(userIdClaim.Value);
        }
    }
}
