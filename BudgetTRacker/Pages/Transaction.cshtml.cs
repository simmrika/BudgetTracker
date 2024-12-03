using BudgetTracker.Service;
using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using BudgetTRacker.Models;
using BudgetTRacker.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BudgetTRacker.Pages
{
    [Authorize]
    public class TransactionModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BankTransactionDataService _bankTransactionDataService;
        private readonly CashTransactionDataService _cashTransactionDataService;
        private readonly IHttpContextAccessor _contextAccessor;

       private readonly AppDbContext _appDbContext;


        [BindProperty]
        public IEnumerable<TransactionDto> Transactions { get; private set; } = new List<TransactionDto>();

        [BindProperty]
        public IEnumerable<CashTransactionDto> CashTransactions { get; private set; } = new List<CashTransactionDto>();

        [BindProperty]
        public IEnumerable<LinkedAccountRequest> linkedAccounts { get; private set; } = new List<LinkedAccountRequest>();

        public TransactionModel(ILogger<IndexModel> logger, BankTransactionDataService bankTransactionDataService, CashTransactionDataService cashTransactionDataService, IHttpContextAccessor contextAccessor,AppDbContext appDbContext)
        {
            _logger = logger;
            _bankTransactionDataService = bankTransactionDataService;
            _cashTransactionDataService = cashTransactionDataService;
            _contextAccessor = contextAccessor;
            _appDbContext = appDbContext;
        }

        public async Task OnGetAsync()
        {
            var userId = GetUserIdFromCookie();

            var linkedaccout=await _appDbContext.LinkedAccount.Where(e=>e.UserID==userId).SingleOrDefaultAsync();



            if (linkedaccout != null)
            {
                Transactions = await _bankTransactionDataService.GetTransactionsByAccountNumberAsync(linkedaccout.AccountNumber);
            }


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
