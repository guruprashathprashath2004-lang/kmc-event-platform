using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IApiService api;

        public RegisterModel(IApiService _api)
        {
            api = _api;
        }

        [BindProperty]
        public RegisterVM RegisterForm { get; set; } = new RegisterVM();

        private IActionResult RedirectByRole(string? role)
        {
            return role switch
            {
                "Admin" => RedirectToPage("/Admin/Index"),
                "Organizer" => RedirectToPage("/Organizer/Index"),
                _ => RedirectToPage("/Index")
            };
        }

        public void OnGet(string role = "Citizen", string? returnUrl = null)
        {
            RegisterForm.Role = role != "Organizer" ? "Citizen" : "Organizer";
            RegisterForm.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPost()
        {
            if (RegisterForm.Role != "Organizer") RegisterForm.Role = "Citizen";

            if (!ModelState.IsValid)
                return Page();

            var (success, token, fullName, role, message) = await api.RegisterAccountAsync(RegisterForm);

            if (!success || token == null)
            {
                RegisterForm.ErrorMessage = message;
                return Page();
            }

            HttpContext.Session.SetString("AuthToken", token);
            HttpContext.Session.SetString("AuthName", fullName ?? RegisterForm.Username);
            HttpContext.Session.SetString("AuthRole", role ?? RegisterForm.Role);

            if (!string.IsNullOrEmpty(RegisterForm.ReturnUrl) && Url.IsLocalUrl(RegisterForm.ReturnUrl))
                return Redirect(RegisterForm.ReturnUrl);

            return RedirectByRole(role ?? RegisterForm.Role);
        }
    }
}
