using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using BudgetTracker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BudgetTRacker.Pages
{
    public class CashTransactionsModel : PageModel
    {
        private readonly CashTransactionDataService _transactionDataService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _appDbContext;

        [BindProperty]
        public CashTransactionDto NewTransaction { get; set; } = new CashTransactionDto();
        public string ErrorMessage { get; set; }

        [BindProperty]
        public List<Category> CategoriesList { get; set; }


        public CashTransactionsModel( CashTransactionDataService transactionDataService, IHttpContextAccessor contextAccessor, AppDbContext appDbContext)
        {
            
            _transactionDataService = transactionDataService;
            _contextAccessor = contextAccessor;
            _appDbContext = appDbContext;
        }
        public async Task OnGetAsync()
        {

            CategoriesList = await _appDbContext.Category.ToListAsync();



        }

        public async Task<IActionResult> OnPostAsync()
        {
           

            NewTransaction.UserId = GetUserIdFromCookie();

            var result = await _transactionDataService.AddTransactionAsync(NewTransaction);
            //if (result)
            //{
            //    LogInformation("New transaction added successfully.");
            //    return RedirectToPage("/Transaction");
            //}
            //else
            //{
            //    LogError("Error adding new transaction.");
            //    ModelState.AddModelError(string.Empty, "An error occurred while adding the transaction.");
            //
            //    
            //}

            return Page(); 
        }

        public int GetUserIdFromCookie()
        {
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
