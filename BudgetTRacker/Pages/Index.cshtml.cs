using BudgetTracker.Service;
using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using BudgetTRacker.Models;
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
        public readonly AccountDetailsDataService _accountDetailsDataService;
        public readonly AddCashDateService _addcashDateService;
        private readonly UserDataService _userDataService;

        private readonly CashTransactionDataService _cashTransactionDataService;
        private readonly ViewSumCashDataService _viewSumCashDataService;
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _contextAccessor;



        [BindProperty]
        public IEnumerable<TransactionDto> Transactions { get; private set; } = new List<TransactionDto>();

        [BindProperty]
        public List<CashEntryDto>? CashEntry { get; private set; } // Single cash entry

        [BindProperty]
        public decimal? SummedCashAmount { get; private set; }

        [BindProperty]
        public List<CategoryExpenditure>  CategoryExpenditureslist { get;  set; }=new List<CategoryExpenditure>();
        [BindProperty]
        public IEnumerable<CashTransactionDto> CashTransactions { get; private set; } = new List<CashTransactionDto>();


        [BindProperty]
        public UserDto? UserDetails { get; private set; }

        public IndexModel(ILogger<IndexModel> logger,CashTransactionDataService cashTransactionDataService1,
            
            AddCashDateService addCashDateService, BankTransactionDataService bankTransactionDataService, AccountDetailsDataService accountDetailsDataService, UserDataService userDataService, ViewSumCashDataService viewSumCashDataService, IHttpContextAccessor contextAccessor, AppDbContext appDbContext)
        {
            _logger = logger;
            _bankTransactionDataService = bankTransactionDataService;
            _accountDetailsDataService = accountDetailsDataService;
            _userDataService = userDataService;
            _appDbContext = appDbContext;
            _contextAccessor = contextAccessor;
            _appDbContext = appDbContext;
            _addcashDateService = addCashDateService;
            _cashTransactionDataService = cashTransactionDataService1;
            _viewSumCashDataService = viewSumCashDataService;

        }

        public async Task OnGetAsync()
        {
            var userId = GetUserIdFromCookie();

            var linkedaccout = await _appDbContext.LinkedAccount.Where(e => e.UserID == userId).SingleOrDefaultAsync();

            var transactionList = new List<TransactionDto>();
            decimal totalIncome = 0, totalExpense = 0;
            decimal bankTotal = 0;

            if (linkedaccout != null)
            {
                var bankTransactions = await _bankTransactionDataService
                        .GetTransactionsByAccountNumberAsync(linkedaccout.AccountNumber);
                transactionList.AddRange(bankTransactions);
                UserDetails = await _accountDetailsDataService.GetUserByAccountNumberAsync(linkedaccout.AccountNumber);

                // Calculate today's income and expense for bank transactions
                foreach (var bankTransaction in bankTransactions)
                {
                    if (bankTransaction.TransactionType == "Deposit")
                    {
                        totalIncome += bankTransaction.Amount;
                        bankTotal += bankTransaction.Amount;
                    }
                    else if (bankTransaction.TransactionType == "Withdraw")
                    {
                        totalExpense += bankTransaction.Amount;
                        bankTotal += bankTransaction.Amount;
                    }
                }

            }


            // Fetch Cash Transactions
            CashTransactions = await _cashTransactionDataService.GetTransactionsByUserIdAsync(userId);

            foreach (var cashTransaction in CashTransactions)
            {
                // Convert CashTransaction to TransactionDto
                TransactionDto transactionDto = new TransactionDto
                {
                    TransactionDate = cashTransaction.Date,
                    Amount = cashTransaction.Total,
                    Notes = cashTransaction.Category.CategoryName,
                    TransactionType = cashTransaction.TransactionType
                };


                transactionList.Add(transactionDto);

                // Since cash transactions are expenses, we add them to totalExpense
                totalExpense += cashTransaction.Total;
            }

            // Assign the combined list back to Transactions
            Transactions = transactionList;


            var bankByCategory = Transactions.GroupBy(t => t.Notes) // Grouping by Notes (could be Category Name or ID)
    .Select(g => new CategoryExpenditure
    {
        TotalAmount = g.Sum(t => t.Amount),
        CategoryName = g.Key // Using the grouped Notes (Category Name)
    })
    .ToList();

            // Add bank transactions grouped by category to the list
            CategoryExpenditureslist.AddRange(bankByCategory);

            // Step 2: Group Cash Transactions by Category (Assuming there's a Notes field)
            var cashByCategory = await _cashTransactionDataService.GetTransactionSumsByCategoryAsync(userId);

            // Add cash transactions grouped by category to the list
            CategoryExpenditureslist.AddRange(cashByCategory);

            // Step 3: Optionally, if you need to sum up all the categories from both bank and cash transactions
            var combinedCategoryExpenditures = CategoryExpenditureslist
                .GroupBy(e => e.CategoryName) // Group by Category Name
                .Select(g => new CategoryExpenditure
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(e => e.TotalAmount) // Sum the amounts from both bank and cash transactions
                })
                .ToList();


            CategoryExpenditureslist=combinedCategoryExpenditures;


            // Fetch user's cash entry and treat it as income
            CashEntry = await _addcashDateService.GetCashEntryByUserIdAsync(userId);
            if (CashEntry != null)
            {
                totalIncome += CashEntry.Select(e=>e.Amount).Sum(); // Add cash entry amount to income
            }

            // Fetch summed cash entry for the user, now just retrieving the TotalAmount
            SummedCashAmount = await _viewSumCashDataService.GetSummedCashAmountByUserIdAsync(userId);


            // Fetch cash entry for the user

            _logger.LogInformation(CashEntry != null
                ? $"Fetched cash entry for user ID: {userId} - Amount: {CashEntry.Select(e => e.Amount).Sum()}, Date: {CashEntry.Select(e => e.Amount).Sum()}"
                : $"No cash entry found for user ID: {userId}");

            // Calculate Total Balance
            decimal totalBalance = 0;

            // Add CashEntry amount if available
            if (CashEntry != null)
            {
                totalBalance += CashEntry.Select(e => e.Amount).Sum(); // Add cash amount
            }

            // Add bank total to total balance
            totalBalance += bankTotal;

            // Set the total balance and today's income/expense to the view
            ViewData["TotalBalance"] = totalBalance;
            ViewData["TodayIncome"] = totalIncome;
            ViewData["TodayExpense"] = totalExpense;
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
