using BudgetTRacker.Data;
using BudgetTRacker.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetTRacker.Pages
{


    [Authorize]

    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly BankTransactionDataService _bankTransactionDataService;
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _contextAccessor;



        [BindProperty]
        public IEnumerable<TransactionDto> Transactions { get; private set; } = new List<TransactionDto>();

        public IndexModel(ILogger<IndexModel> logger, BankTransactionDataService bankTransactionDataService, IHttpContextAccessor contextAccessor, AppDbContext appDbContext)
        {
            _logger = logger;
            _bankTransactionDataService = bankTransactionDataService;
            _contextAccessor = contextAccessor;
            _appDbContext = appDbContext;
        }

        public async Task OnGetAsync()
        {
            var userId = GetUserIdFromCookie();

            var linkedaccout = await _appDbContext.LinkedAccount.Where(e => e.UserID == userId).SingleOrDefaultAsync();



            if (linkedaccout != null)
            {
                Transactions = await _bankTransactionDataService.GetTransactionsByAccountNumberAsync(linkedaccout.AccountNumber);
            }

            Transactions = await _bankTransactionDataService.GetTransactionsByUserIdAsync(1);
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
