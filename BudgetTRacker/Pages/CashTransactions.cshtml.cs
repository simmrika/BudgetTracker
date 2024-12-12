using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using BudgetTracker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace BudgetTRacker.Pages
{
    public class CashTransactionsModel : PageModel
    {
        private readonly CashTransactionDataService _transactionDataService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<CashTransactionsModel> _logger;

        [BindProperty]
        public CashTransactionDto NewTransaction { get; set; } = new CashTransactionDto();
        public string ErrorMessage { get; set; }

        [BindProperty]
        public List<Category> CategoriesList { get; set; }

        public CashTransactionsModel(CashTransactionDataService transactionDataService, 
                                     IHttpContextAccessor contextAccessor,
                                     AppDbContext appDbContext,
                                     ILogger<CashTransactionsModel> logger)
        {
            _transactionDataService = transactionDataService;
            _contextAccessor = contextAccessor;
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            // Fetch categories for the dropdown or selection
            CategoriesList = await _appDbContext.Category.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Set the UserId from the cookie (ensure user is authenticated)





            NewTransaction.UserId = GetUserIdFromCookie();

            // Attempt to add the new transaction
            bool doesexceeds = await CheckTransactionLimitAsync(NewTransaction.UserId, NewTransaction.CategoryId, NewTransaction.Total);


            var myamount=await _appDbContext.SummedCashEntries
                .Where(e => e.UserId == NewTransaction.UserId)  // Filter by UserId
                .Select(e => e.CurrentBalance)      // Only select the TotalAmount field
                .SingleOrDefaultAsync();

            if (NewTransaction.Total > myamount)
            {

                TempData["Error"] = "The transaction is more the cash in hand";
                return RedirectToPage("/CashTransactions");
            }


            if (doesexceeds)
            {
                TempData["Error"] = "The transaction Exceeds the limit";
                return RedirectToPage("/CashTransactions");
            }



            var result = await _transactionDataService.AddTransactionAsync(NewTransaction);

            if (result)
            {
                // Log success and redirect to the transaction page
                _logger.LogInformation("New transaction added successfully.");
                return RedirectToPage("/Transaction");
            }
            else
            {
                // Log the error and display a message
                _logger.LogError("Error adding new transaction.");
                ModelState.AddModelError(string.Empty, "An error occurred while adding the transaction.");

                // Return to the same page with an error message
                return Page();
            }
        }
        public async Task<bool> CheckTransactionLimitAsync(int userId, int categoryId, decimal amount)
        {
            // Define the parameters
            var userIdParam = new SqlParameter("@UserId", userId);
            var categoryIdParam = new SqlParameter("@CategoryId", categoryId);
            var amountParam = new SqlParameter("@Amount", amount);

            // Define the output parameter
            var isExceededParam = new SqlParameter
            {
                ParameterName = "@IsExceeded",
                SqlDbType = System.Data.SqlDbType.Bit,
                Direction = System.Data.ParameterDirection.Output
            };

            // Execute the stored procedure
            await _appDbContext.Database.ExecuteSqlRawAsync(
                "EXEC CheckTransactionLimit @UserId, @CategoryId, @Amount, @IsExceeded OUTPUT",
                userIdParam, categoryIdParam, amountParam, isExceededParam);

            // Retrieve the output value
            return (bool)isExceededParam.Value;
        }


        public int GetUserIdFromCookie()
        {
            // Get the UserId from the authenticated user's claims
            var user = _contextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new Exception("User ID claim not found in the cookie.");
            }

            return int.Parse(userIdClaim.Value);
        }
    }
}
