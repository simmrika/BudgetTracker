using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetTRacker.Pages
{
    public class LogOutModel : PageModel
    {
        // POST handler to log the user out
        public async Task<IActionResult> OnPostAsync()
        {
            // Sign the user out using ASP.NET Core Identity
            await HttpContext.SignOutAsync();

            // Redirect to the home page or login page after logging out
            return RedirectToPage("/SignIn"); 
        }
    }
}
