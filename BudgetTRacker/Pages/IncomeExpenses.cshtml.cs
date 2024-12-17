using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BudgetTRacker.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Service;
using BudgetTRacker.Data;
using Microsoft.EntityFrameworkCore;
using BudgetTRacker.Entities;

namespace BudgetTRacker.Pages
{
    [Authorize]
    public class IncomeExpensesModel : PageModel
    {
        private readonly ILogger<IncomeExpensesModel> _logger;
        private readonly CashTransactionDataService _cashTransactionDataService;
        private readonly BankTransactionDataService _bankTransactionDataService;
        private readonly AddCashDateService _cashDateService;
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _contextAccessor;



        public IncomeExpensesModel(
            ILogger<IncomeExpensesModel> logger,
            CashTransactionDataService cashTransactionDataService,
            BankTransactionDataService bankTransactionDataService,
            AddCashDateService cashDateService, AppDbContext appDbContext, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _cashTransactionDataService = cashTransactionDataService;
            _bankTransactionDataService = bankTransactionDataService;
            _cashDateService = cashDateService;
            _appDbContext = appDbContext;
            _contextAccessor = contextAccessor;
        }

        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal TotalBalance { get; set; }

        public List<ChartDto> WeeklyIncomeExpenseData { get; set; } = new();
        public List<ChartDto> MonthlyIncomeExpenseData { get; set; } = new();

        private DateTime GetWeekEnd(DateTime date)
        {
            var startOfWeek = GetWeekStart(date);
            return startOfWeek.AddDays(6); // End of the week (Sunday)
        }
        public async Task OnGetAsync()
        {
            var userId = GetUserIdFromCookie();
            TotalIncome = 0;
            TotalExpense = 0;
            TotalBalance = 0;

            // Fetch all cash transactions and cash entry
            var cashTransactions = await _cashTransactionDataService.GetTransactionsByUserIdAsync(userId);
            var cashEntry = await _cashDateService.GetCashEntryByUserIdAsync(userId);

            if (cashEntry != null)
            {
                TotalIncome += cashEntry.Select(e => e.Amount).Sum() ; // Cash entry as income
                TotalBalance += cashEntry.Select(e => e.Amount).Sum();
            } 

            // Process cash transactions
            foreach (var cashTransaction in cashTransactions)
            {
                if (cashTransaction.TransactionType == "Cash Out")
                {
                    TotalExpense += cashTransaction.Total;
                }
  
            }

            // Fetch and process bank transactions
            var linkedaccout = await _appDbContext.LinkedAccount.Where(e => e.UserID == userId).SingleOrDefaultAsync();
            if (linkedaccout != null)
            {
                var bankTransactions = await _bankTransactionDataService.GetTransactionsByAccountNumberAsync(linkedaccout.AccountNumber);
                foreach (var bankTransaction in bankTransactions)
                {
                    if (bankTransaction.TransactionType == "Deposit")
                    {
                        TotalIncome += bankTransaction.Amount;
                        TotalBalance += bankTransaction.Amount;
                    }
                    else if (bankTransaction.TransactionType == "Withdraw")
                    {
                        TotalExpense += bankTransaction.Amount;
                        TotalBalance -= bankTransaction.Amount;
                    }
                }
            }

            // Combine all transactions for grouping
            var allTransactions = cashTransactions
                .Select(ct => new { Date = ct.Date, Total = ct.Total, TransactionType = ct.TransactionType })
                .Concat(
                    linkedaccout == null
                        ? Enumerable.Empty<dynamic>()
                        : (await _bankTransactionDataService
                                .GetTransactionsByAccountNumberAsync(linkedaccout.AccountNumber))
                            .Select(bt => new
                            {
                                Date = bt.TransactionDate, // Normalize to `Date`
                                Total = bt.Amount,         // Normalize to `Total`
                                TransactionType = bt.TransactionType
                            }));




            // Group transactions by day of the week and calculate totals
            WeeklyIncomeExpenseData = allTransactions
                .Where(t => t.Date >= GetWeekStart(DateTime.Now) && t.Date <= GetWeekEnd(DateTime.Now)) // Filter for the current week
                .GroupBy(t => t.Date.DayOfWeek) // Group by day of the week
                .Select(g => new ChartDto
                {
                    TimePeriod = g.Key.ToString(), // "Sunday", "Monday", etc.
                    Income = g.Where(t => t.TransactionType == "Deposit" || t.TransactionType == "Income"  || t.TransactionType == "Cash In")
                              .Sum(t => (decimal)t.Total), // Total Income for the day
                    Expenses = g.Where(t => t.TransactionType == "Withdraw" || t.TransactionType == "Expense" || t.TransactionType=="Cash Out")
                                .Sum(t => (decimal)t.Total) // Total Expenses for the day
                })
                .OrderBy(c => Enum.Parse<DayOfWeek>(c.TimePeriod)) // Ensure days are in correct order
                .ToList();


            // Group transactions by month and calculate totals
            MonthlyIncomeExpenseData = allTransactions
        .GroupBy(t => new { t.Date.Year, t.Date.Month })
        .Select(g => new ChartDto
        {
            
            TimePeriod= new DateTime(1, g.Key.Month, 1).ToString("MMMM"),
      
          Income= g.Where(t => t.TransactionType == "Deposit" || t.TransactionType == "Income" || t.TransactionType=="Cash In")
         .Sum(t => (decimal)t.Total), // Explicitly cast to decimal
          Expenses= g.Where(t => t.TransactionType == "Withdraw" || t.TransactionType == "Expense" || t.TransactionType == "Cash Out")
         .Sum(t => (decimal)t.Total)  // Explicitly cast to decimal
        })
        .ToList();

            _logger.LogInformation($"Total Income: {TotalIncome}, Total Expense: {TotalExpense}, Total Balance: {TotalBalance}");
        }

        private DateTime GetWeekStart(DateTime date)
        {
            var diff = date.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            return date.AddDays(-1 * diff).Date;
        }

        private int GetUserIdFromCookie()
        {
            var user = HttpContext?.User;

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
