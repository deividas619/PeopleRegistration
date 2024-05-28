using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonRegistration.BusinessLogic.Interfaces;

namespace PersonRegistration.UI.Pages
{
    public class RegisterModel(IUserService userService) : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = userService.Register(Username, Password);
            if (!response.IsSuccess)
            {
                Message = response.Message;
                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}
