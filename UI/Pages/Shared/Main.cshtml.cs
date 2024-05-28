using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PersonRegistration.UI.Pages.Shared
{
    [Authorize]
    public class MainModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
