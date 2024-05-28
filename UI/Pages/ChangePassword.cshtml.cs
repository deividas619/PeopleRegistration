using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonRegistration.BusinessLogic.Interfaces;

namespace PersonRegistration.UI.Pages
{
    [Authorize]
    public class ChangePasswordModel(IUserService userService) : PageModel
    {
        [BindProperty]
        public string OldPassword { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string NewPasswordAgain { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var response = userService.ChangeUserPassword(username, OldPassword, NewPassword, NewPasswordAgain);

            if (!response.IsSuccess)
            {
                Message = response.Message;
                return Page();
            }

            Response.Cookies.Delete("jwtToken");

            return RedirectToPage("/Index");
        }
    }
}
