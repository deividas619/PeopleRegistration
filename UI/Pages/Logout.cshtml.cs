using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PersonRegistration.UI.Pages
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            Response.Cookies.Delete("jwtToken");

            return RedirectToPage("/Index");
        }
    }
}
