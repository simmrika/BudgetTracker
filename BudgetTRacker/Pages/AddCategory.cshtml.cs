using BudgetTracker.Service; // Ensure you have the necessary namespace for CategoryDto and CategoryDataService
using BudgetTRacker.Data;
using BudgetTRacker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudgetTRacker.Pages
{
    public class AddCategoryModel : PageModel
    {
       
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        [BindProperty]
        public CategoryLimit categoryLimit { get; set; }
        // Bind the category data to the form
  
        public List<Category> CategoriesList { get; set; }


        // Inject the CategoryDataService into the constructor
        public AddCategoryModel(AppDbContext appDbContext, IHttpContextAccessor contextAccessor)
        {
            _appDbContext = appDbContext;
            _contextAccessor = contextAccessor;
        }




        public async Task OnGetAsync()
        {

            CategoriesList = await _appDbContext.Category.ToListAsync();

        }


        // POST handler to save the new category
        public async Task<IActionResult> OnPostAsync()
        {

            categoryLimit.UserId = GetUserIdFromCookie();
            if (ModelState.IsValid)
            {




                await _appDbContext.CategoryLimit.AddAsync(categoryLimit);

                await _appDbContext.SaveChangesAsync();

             
                    return RedirectToPage("/Index");
            
            }
            return Page(); // Stay on the same page if there was an error or invalid data
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
