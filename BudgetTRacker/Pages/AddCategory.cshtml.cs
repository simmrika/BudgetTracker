using BudgetTracker.Service; // Ensure you have the necessary namespace for CategoryDto and CategoryDataService
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BudgetTRacker.Pages
{
    public class AddCategoryModel : PageModel
    {
        private readonly CategoryDataService _categoryService;

        // Bind the category data to the form
        [BindProperty]
        public CategoryDto Category { get; set; } = new CategoryDto();

        // Inject the CategoryDataService into the constructor
        public AddCategoryModel(CategoryDataService categoryService)
        {
            _categoryService = categoryService;
        }



        // POST handler to save the new category
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.AddCategoryAsync(Category);

                if (result)
                {
                    // Redirect to the Categories page after adding the category successfully
                    return RedirectToPage("/Category");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add category.");
                }
            }
            return Page(); // Stay on the same page if there was an error or invalid data
        }
    }
}
