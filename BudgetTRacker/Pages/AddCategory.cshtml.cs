using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BudgetTracker.Service;
using System.Threading.Tasks;
using BudgetTRacker.Service;

namespace BudgetTRacker.Pages
{
    public class AddCategoryModel : PageModel
    {
        private readonly AddCategoryDataService _addcategoryDataService;

        [BindProperty]
        public AddCategoryDto AddCategory { get; set; } = new AddCategoryDto();

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }


        public AddCategoryModel(AddCategoryDataService addcategoryDataService)
        {
            _addcategoryDataService = addcategoryDataService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields.";
                return Page();
            }

            try
            {
                bool result = await _addcategoryDataService.AddCategoryAsync(AddCategory.CategoryName);

                if (result)
                {
                    TempData["Success"] = "New Category added successfully!";
                    SuccessMessage = "Category added successfully.";
                    return RedirectToPage("/Category"); // Redirect to the Categories page
                }
                else
                {
                    ErrorMessage = "An error occurred while adding the category.";
                    return Page();
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
