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
using System.Text.Json;

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

        public string SerializedTransactions => JsonSerializer.Serialize(Transactions);

        public string SerializedCashTransactions => JsonSerializer.Serialize(CashTransactions);


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

            var linkedAccount = await _appDbContext.LinkedAccount
                .Where(e => e.UserID == userId)
                .SingleOrDefaultAsync();

            var transactionsList = new List<TransactionDto>();

            if (linkedAccount != null)
            {
                var bankTransactions = await _bankTransactionDataService
                    .GetTransactionsByAccountNumberAsync(linkedAccount.AccountNumber);
                transactionsList.AddRange(bankTransactions);
            }

            CashTransactions = await _cashTransactionDataService.GetTransactionsByUserIdAsync(userId);

            foreach (var cashTransaction in CashTransactions)
            {
                TransactionDto transactionDto = new TransactionDto
                {
                    TransactionDate = cashTransaction.Date,
                    Amount = cashTransaction.Total,
                    Notes = cashTransaction.Category.CategoryName,
                    TransactionType = cashTransaction.TransactionType
                };

                // Check if the transaction date is greater than today's date and set the status accordingly
                transactionDto.Status = transactionDto.TransactionDate.Date > DateTime.Today ? "Pending" : "Success";

                transactionsList.Add(transactionDto);
            }

            // Assign the combined list back to Transactions
            Transactions = transactionsList;
        }


        public async Task<IActionResult> OnPostDeleteAsync(List<int> selectedTransactionIds)
        {
            if (selectedTransactionIds == null || selectedTransactionIds.Count == 0)
            {
                // Handle no transactions selected
                TempData["Error"] = "No transactions selected for deletion.";
                return RedirectToPage();
            }

            var userId = GetUserIdFromCookie();

            // Delete selected Cash Transactions
            var cashTransactionsToDelete = await _appDbContext.CashTransaction
                .Where(t => selectedTransactionIds.Contains(t.Id) && t.UserId == userId)
                .ToListAsync();

            if (cashTransactionsToDelete.Any())
            {
                _appDbContext.CashTransaction.RemoveRange(cashTransactionsToDelete);
            }


            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Selected transactions deleted successfully.";
            return RedirectToPage();
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
