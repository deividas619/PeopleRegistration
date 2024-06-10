using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeopleRegistration.BusinessLogic.Interfaces;

namespace PeopleRegistration.UI.Pages
{
    public class IndexModel(IUserService userService, IJwtService jwtService) : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = userService.Login(Username, Password);

            if (!response.IsSuccess)
            {
              Message = response.Message;
              return Page();
            }

            var jwtToken = jwtService.GetJwtToken(Username, response.Role);

            Response.Cookies.Append("jwtToken", jwtToken, new CookieOptions { HttpOnly = true, Secure = true });

            return RedirectToPage("/Shared/Main");
        }
    }
}
