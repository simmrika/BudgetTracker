using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetTRacker.Pages
{
    [Authorize]
    public class SignInModel : PageModel
    {

        public void OnGet()
        {
        }
    }
}
