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
        public readonly AddCashDateService _cashDateService;
        private readonly UserDataService _userDataService;

        private readonly CashTransactionDataService _cashTransactionDataService;

        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _contextAccessor;



        [BindProperty]
        public IEnumerable<TransactionDto> Transactions { get; private set; } = new List<TransactionDto>();

        [BindProperty]
        public CashEntryDto? CashEntry { get; private set; } // Single cash entry

        [BindProperty]
        public List<CategoryExpenditure>  CategoryExpenditureslist { get;  set; }=new List<CategoryExpenditure>();


        [BindProperty]
        public UserDto? UserDetails { get; private set; }

        public IndexModel(ILogger<IndexModel> logger,CashTransactionDataService cashTransactionDataService1,
            
            AddCashDateService addcash, BankTransactionDataService bankTransactionDataService, AccountDetailsDataService accountDetailsDataService, UserDataService userDataService, AddCashDateService addCashDateService, IHttpContextAccessor contextAccessor, AppDbContext appDbContext)
        {
            _logger = logger;
            _bankTransactionDataService = bankTransactionDataService;
            _accountDetailsDataService = accountDetailsDataService;
            _userDataService = userDataService;
            _appDbContext = appDbContext;
            _contextAccessor = contextAccessor;
            _appDbContext = appDbContext;
            _cashDateService = addcash;
            _cashTransactionDataService = cashTransactionDataService1;
        }

        public async Task OnGetAsync()
        {
            var userId = GetUserIdFromCookie();

            var linkedaccout = await _appDbContext.LinkedAccount.Where(e => e.UserID == userId).SingleOrDefaultAsync();






         



            if (linkedaccout != null)
            {
                Transactions = await _bankTransactionDataService.GetTransactionsByAccountNumberAsync(linkedaccout.AccountNumber);
                UserDetails = await _accountDetailsDataService.GetUserByAccountNumberAsync(linkedaccout.AccountNumber);

            }






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



            var user = await _userDataService.GetUserByIdAsync(userId); // Get user by ID

                                                                        // Fetch cash entry for the user
            CashEntry = await _cashDateService.GetCashEntryByUserIdAsync(userId);

            _logger.LogInformation(CashEntry != null
                ? $"Fetched cash entry for user ID: {userId} - Amount: {CashEntry.Amount}, Date: {CashEntry.AddDate}"
                : $"No cash entry found for user ID: {userId}");
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
