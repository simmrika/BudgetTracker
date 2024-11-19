using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetTRacker.Pages
{
    [Authorize]
    public class AddAccountModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
