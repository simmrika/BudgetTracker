using BudgetTracker.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace BudgetTRacker.Pages
{
    [Authorize]
    public class CategoryModel : PageModel
    {
        private readonly CategoryDataService _categoryService;
        private readonly IHttpContextAccessor _contextAccessor;

        public CategoryModel(CategoryDataService categoryService, IHttpContextAccessor contextAccessor)
        {
            _categoryService = categoryService;
            _contextAccessor = contextAccessor;
        }

        [BindProperty]
        public List<CategoryDto> Category { get; set; } = new List<CategoryDto>();

        [BindProperty]
        public List<int> SelectedCategories { get; set; } = new List<int>();


        public async Task OnGetAsync()
        {
            var userId = GetUserIdFromCookie();
            Category = (await _categoryService.GetCategoriesByUserIdAsync(userId)).ToList();
        }

        public async Task<IActionResult> OnPostDeleteSelectedAsync()
        {
            if (SelectedCategories != null && SelectedCategories.Any())
            {
                // Call the service to delete categories by their IDs
                var result = await _categoryService.DeleteCategoriesAsync(SelectedCategories);

                if (!result)
                {
                    ModelState.AddModelError("", "Failed to delete selected categories.");
                }
                else
                {
                    // Optionally, re-fetch the categories after deletion
                    var userId = GetUserIdFromCookie();
                    Category = (await _categoryService.GetCategoriesByUserIdAsync(userId)).ToList();
                }
            }

            return RedirectToPage(); // Redirect to the same page after the operation
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
