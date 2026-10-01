using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IApiService api;

        public LoginModel(IApiService _api)
        {
            api = _api;
        }

        [BindProperty]
        public LoginVM LoginForm { get; set; } = new LoginVM();

        private IActionResult RedirectByRole(string? role)
        {
            return role switch
            {
                "Admin" => RedirectToPage("/Admin/Index"),
                "Organizer" => RedirectToPage("/Organizer/Index"),
                _ => RedirectToPage("/Index")
            };
        }

        public IActionResult OnGet(string? returnUrl = null)
        {
            var role = HttpContext.Session.GetString("AuthRole");
            if (!string.IsNullOrEmpty(role))
                return RedirectByRole(role);

            LoginForm.ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var (success, token, fullName, role, message) = await api.LoginAsync(LoginForm.Username, LoginForm.Password);

            if (!success || token == null)
            {
                LoginForm.ErrorMessage = message;
                return Page();
            }

            HttpContext.Session.SetString("AuthToken", token);
            HttpContext.Session.SetString("AuthName", fullName ?? LoginForm.Username);
            HttpContext.Session.SetString("AuthRole", role ?? "Citizen");

            if (!string.IsNullOrEmpty(LoginForm.ReturnUrl) && Url.IsLocalUrl(LoginForm.ReturnUrl))
                return Redirect(LoginForm.ReturnUrl);

            return RedirectByRole(role);
        }
    }
}
