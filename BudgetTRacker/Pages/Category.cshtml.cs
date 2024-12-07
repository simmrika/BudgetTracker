using BudgetTRacker.Service;
using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetTRacker.Models;
using BudgetTracker.Service;

namespace BudgetTRacker.Pages
{
    [Authorize]
    public class CategoryModel : PageModel
    {
        private readonly ILogger<CategoryModel> _logger;
        private readonly BankTransactionDataService _bankTransactionDataService;
        private readonly CashTransactionDataService _cashTransactionDataService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _appDbContext;

        [BindProperty]
        public List<CategoryExpenditure> CategoryExpenditureslist { get; private set; } = new List<CategoryExpenditure>();

        [BindProperty]
        public List<string> SelectedCategories { get; set; } = new List<string>();

        public CategoryModel(
            ILogger<CategoryModel> logger,
            BankTransactionDataService bankTransactionDataService,
            CashTransactionDataService cashTransactionDataService,
            IHttpContextAccessor contextAccessor,
            AppDbContext appDbContext)
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

            // Fetch linked account
            var linkedAccount = await _appDbContext.LinkedAccount
                .Where(e => e.UserID == userId)
                .SingleOrDefaultAsync();

            if (linkedAccount != null)
            {
                // Fetch bank transactions by account number
                var bankTransactions = await _bankTransactionDataService.GetTransactionsByAccountNumberAsync(linkedAccount.AccountNumber);

                // Group bank transactions by category (Notes) and calculate totals
                var bankByCategory = bankTransactions.GroupBy(t => t.Notes) // Group by Notes
                    .Select(g => new CategoryExpenditure
                    {
                        CategoryName = g.Key, // Category Name
                        TotalAmount = g.Sum(t => t.Amount) // Sum of amounts
                    })
                    .ToList();

                CategoryExpenditureslist.AddRange(bankByCategory);
            }

            // Fetch and group cash transactions by category
            var cashByCategory = await _cashTransactionDataService.GetTransactionSumsByCategoryAsync(userId);

            CategoryExpenditureslist.AddRange(cashByCategory);

            // Combine and merge categories
            var combinedCategoryExpenditures = CategoryExpenditureslist
                .GroupBy(e => e.CategoryName) // Group by Category Name
                .Select(g => new CategoryExpenditure
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(e => e.TotalAmount) // Sum amounts for combined categories
                })
                .ToList();

            CategoryExpenditureslist = combinedCategoryExpenditures;

            _logger.LogInformation("Successfully fetched and processed category expenditures.");
        }



        private int GetUserIdFromCookie()
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
