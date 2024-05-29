using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PeopleRegistration.UI.Pages.Shared
{
    [Authorize]
    public class MainModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
